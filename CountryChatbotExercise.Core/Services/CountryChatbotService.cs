using CountryChatbotExercise.Core.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CountryChatbotExercise.Core.Services
{
    public class CountryChatbotService : ICountryChatbotService
    {
        private readonly MessageProcessor _messageProcessor;
        private readonly ILogger<CountryChatbotService> _logger;

        private int MostRecentId { get; set; }
        private IDictionary<int, Conversation> Conversations { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CountryChatbotService"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public CountryChatbotService(ILogger<CountryChatbotService> logger)
        {
            _messageProcessor = new MessageProcessor();
            MostRecentId = 0;
            Conversations = new Dictionary<int, Conversation>();
            _logger = logger;
            _logger.LogInformation("CountryChatbotService created.");
        }

        /// <summary>
        /// Generates the answer to the user message asynchronously.
        /// </summary>
        /// <param name="userMessage">The user message.</param>
        /// <returns>
        /// The <see cref="Message">Message</see> answering the user Message
        /// </returns>
        public async Task<Message> GenerateAnswerAsync(Message userMessage)
        {
            _logger.LogInformation("Received message: '{userMessage.Body}'", userMessage.Body);

            var isNewConversation = AssignConversationId(userMessage);
            var answer = await _messageProcessor.ProcessMessageAsync(userMessage, isNewConversation);

            var id = userMessage.ConversationId.Value;

            if (answer.Status == MessageStatus.End)
            {
                EndConversation(id);
            }
            else
            {
                var conversationHistory = Conversations[id].Messages;
                conversationHistory.Add(userMessage);
                conversationHistory.Add(answer);
            }

            _logger.LogInformation("Replied with: '{answerBody}'", answer.Body);

            return answer;
        }

        private bool AssignConversationId(Message userMessage)
        {
            if (userMessage.ConversationId.HasValue)
            {
                return false;
            }

            var newId = StartNewConversation();
            userMessage.ConversationId = newId;
            return true;
        }

        /// <summary>
        /// Starts the new conversation.
        /// </summary>
        /// <returns>
        /// The conversation identifier.
        /// </returns>
        public int StartNewConversation()
        {
            var id = MostRecentId++;

            var conversation = new Conversation(id);
            Conversations.Add(id, conversation);

            _logger.LogInformation("Started new conversation (id: {id})", id);

            return id;
        }

        /// <summary>
        /// Ends the conversation.
        /// </summary>
        /// <param name="id">The conversation identifier.</param>
        public void EndConversation(int id)
        {
            _logger.LogInformation("Trying to end conversation id: {id}...", id);
            if (Conversations.ContainsKey(id))
            {
                Conversations.Remove(id);
                _logger.LogInformation("Conversation id: {id} ended.", id);
            }
        }
    }
}
