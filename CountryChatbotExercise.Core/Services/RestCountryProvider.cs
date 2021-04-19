using CountryChatbotExercise.Core.Models.Country;
using RestSharp;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CountryChatbotExercise.Core.Services
{
    internal class RestCountryProvider
    {
        private static readonly string RestCountriesBaseUri = "https://restcountries.eu/rest/v2/";

        private readonly Dictionary<string, RestCountry> _cache;

        private IEnumerable<string> _countryNames;

        public RestCountryProvider()
        {
            _cache = new Dictionary<string, RestCountry>();
        }

        /// <summary>
        /// Gets all country names asynchronously.
        /// </summary>
        /// <returns>All country names.</returns>
        public async Task<IEnumerable<string>> GetCountryNamesAsync()
        {
            if (_countryNames != null && _countryNames.Any())
            {
                return _countryNames;
            }

            var endpoint = "all?fields=name";
            var countries = await RequestRestCountryEndpointAsync<IEnumerable<RestCountry>>(endpoint);

            if (countries == null)
            {
                return new string[] { };
            }

            var names = countries.Select(country => country.Name);

            _countryNames = names;

            return names;
        }

        /// <summary>
        /// Gets the specific country asynchronously.
        /// </summary>
        /// <param name="countryName">Name of the country.</param>
        /// <returns>The <see cref="RestCountry"/> matching the <paramref name="countryName"/>.</returns>
        public async Task<RestCountry> GetCountryAsync(string countryName)
        {
            if (_cache.ContainsKey(countryName))
            {
                return _cache[countryName];
            }

            var endpoint = $"name/{countryName}";
            var results = await RequestRestCountryEndpointAsync<IEnumerable<RestCountry>>(endpoint);

            if (results == null)
            {
                return null;
            }

            var result = results.FirstOrDefault();

            _cache.Add(countryName, result);

            return result;
        }

        /// <summary>
        /// Requests the rest country endpoint asynchronously.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="endpoint">The endpoint name.</param>
        /// <returns></returns>
        private Task<T> RequestRestCountryEndpointAsync<T>(string endpoint) where T : class
        {
            try
            {
                // According to https://stackoverflow.com/a/49588931/5832956,
                // it's better to create a new instance of RestClient for each request.
                var client = new RestClient(RestCountriesBaseUri);
                var request = new RestRequest(endpoint, Method.GET, DataFormat.Json);
                return client.GetAsync<T>(request);
            }
            catch
            {
                // TODO: Figure out the different exceptions that could occur here and handle them gracefully.
                // According to the documentation, RestSharp does not typically throw exceptions,
                // at least not for synchronous non-generic methods.
                // Since we are using an async generic method, we should expect issues here.
            }

            return Task.FromResult<T>(null); // FIXME
        }
    }
}
