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
            // TODO: This method is ugly and confusing. It should be refactored.
            _logger.LogInformation("Received message: '{userMessage.Body}'", userMessage.Body);

            int id;
            var newConversation = false;
            if (!userMessage.ConversationId.HasValue)
            {
                id = StartNewConversation();
                userMessage.ConversationId = id;

                newConversation = true;
            }
            else
            {
                id = userMessage.ConversationId.Value;
            }

            var isGoodbye = userMessage.Body.ToLower().Contains("goodbye");

            string answerBody;
            if (isGoodbye)
            {
                answerBody = "Goodbye!";
            }
            else
            {
                answerBody = await _messageProcessor.ProcessMessageAsync(userMessage.Body, newConversation);
            }

            var answer = new Message()
            {
                ConversationId = id,
                MessageType = MessageType.Bot,
                Timestamp = DateTime.Now,
                Body = answerBody
            };

            if (isGoodbye)
            {
                answer.ConversationId = null;
                EndConversation(id);
            }
            else
            {
                Conversations[id].Messages.Add(answer);
            }

            _logger.LogInformation("Replied with: '{answerBody}'", answerBody);

            return answer;
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
