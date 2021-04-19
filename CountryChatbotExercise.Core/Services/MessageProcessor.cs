using CountryChatbotExercise.Core.Helpers;
using CountryChatbotExercise.Core.Models.Country;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CountryChatbotExercise.Core.Services
{
    public class MessageProcessor
    {
        private readonly RestCountryProvider _restCountryProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageProcessor"/> class.
        /// </summary>
        public MessageProcessor()
        {
            _restCountryProvider = new RestCountryProvider();
        }

        /// <summary>
        /// Processes the message asynchronously.
        /// </summary>
        /// <param name="body">The message body.</param>
        /// <param name="newConversation">if set to <c>true</c>, starts the answer with a hearty 'Hello!'.</param>
        /// <returns>The answering message body.</returns>
        public async Task<string> ProcessMessageAsync(string body, bool newConversation)
        {
            var words = Regex.Split(body, "\\W");

            var countryNames = await _restCountryProvider.GetCountryNamesAsync();
            var bestCountryName = CalculateBestCountryName(words, countryNames);
            var country = await _restCountryProvider.GetCountryAsync(bestCountryName);

            if (country == null)
            {
                return "I am sorry, I could not connect to the external data provider. Please try again later.";
            }

            var sentence = GenerateSentence(country, 3);
            var answer = $"Did you know that {country.Name}'s {sentence}?";

            if (newConversation)
            {
                answer = $"Hello! {answer}";
            }

            return answer;
        }

        /// <summary>
        /// Generates the sentence out of random <see cref="RestCountry"/> properties.
        /// </summary>
        /// <param name="country">The country.</param>
        /// <param name="propertyCount">The property count.</param>
        /// <returns>The generated sentence.</returns>
        private string GenerateSentence(RestCountry country, int propertyCount)
        {
            var propertyPairs = RestCountryPropertyInfoProvider.GetRandomProperties(country, 3);
            var sentencifiedPairs = propertyPairs.Select(pair => $"{pair.PropertyName} is {pair.Value}");
            var joinedSentence = string.Join(", ", sentencifiedPairs.Take(propertyCount - 1));
            joinedSentence += $" and {sentencifiedPairs.Last()}";

            return joinedSentence;
        }

        /// <summary>
        /// Calculates the name of the most probably country when compared to the words passed within the message body (using Levenshtein algorithm).
        /// </summary>
        /// <param name="words">The words contained within the message.</param>
        /// <param name="countries">The country names.</param>
        /// <returns>Most probably country name.</returns>
        /// <exception cref="ArgumentNullException">
        /// words
        /// or
        /// countries
        /// </exception>
        private string CalculateBestCountryName(IEnumerable<string> words, IEnumerable<string> countries)
        {
            if (words == null)
            {
                throw new ArgumentNullException(nameof(words));
            }

            if (countries == null)
            {
                throw new ArgumentNullException(nameof(countries));
            }

            // TODO: I am 100% sure there should be a more elegant way to handle this...
            var bestCountryName = "";
            var lowestDistance = int.MaxValue;
            foreach (var word in words)
            {
                foreach (var countryCandidate in countries)
                {
                    var distance = LevenshteinDistance.Calculate(word, countryCandidate);
                    if (distance < lowestDistance)
                    {
                        lowestDistance = distance;
                        bestCountryName = countryCandidate;
                    }
                }
            }

            return bestCountryName;
        }
    }
}