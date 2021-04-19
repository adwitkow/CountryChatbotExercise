using CountryChatbotExercise.Core.Models;
using System.Threading.Tasks;

namespace CountryChatbotExercise.Core.Services
{
    public interface ICountryChatbotService
    {
        /// <summary>
        /// Generates the answer to the user message asynchronously.
        /// </summary>
        /// <param name="userMessage">The user message.</param>
        /// <returns>The <see cref="Message">Message</see> answering the user Message</returns>
        Task<Message> GenerateAnswerAsync(Message userMessage);

        /// <summary>
        /// Starts the new conversation.
        /// </summary>
        /// <returns>The conversation identifier.</returns>
        int StartNewConversation();

        /// <summary>
        /// Ends the conversation.
        /// </summary>
        /// <param name="id">The conversation identifier.</param>
        void EndConversation(int id);
    }
}
