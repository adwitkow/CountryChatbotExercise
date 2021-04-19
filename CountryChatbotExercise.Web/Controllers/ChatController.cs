using CountryChatbotExercise.Core.Models;
using CountryChatbotExercise.Core.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CountryChatbotExercise.Web.Controllers
{
    /// <summary>
    /// Controller responsible for client-server chat communication
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : Controller
    {
        private readonly ICountryChatbotService _chatbot;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChatController"/> class.
        /// </summary>
        /// <param name="chatbot">The chatbot.</param>
        public ChatController(ICountryChatbotService chatbot)
        {
            _chatbot = chatbot;
        }

        /// <summary>
        /// Posts the user chat message to the repository and generates an answer..
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>An answer to the user message.</returns>
        [HttpPost("PostMessage")]
        public async Task<IActionResult> PostMessage([FromBody] Message message)
        {
            var result = await _chatbot.GenerateAnswerAsync(message);
            return Json(result);
        }

        /// <summary>
        /// Starts the conversation.
        /// </summary>
        /// <returns>The conversation identifier.</returns>
        [HttpPost("StartConversation")]
        public IActionResult StartConversation()
        {
            var id = _chatbot.StartNewConversation();
            return Json(new { ConversationId = id });
        }

        /// <summary>
        /// Ends the conversation.
        /// </summary>
        /// <param name="conversationId">The conversation identifier.</param>
        /// <returns></returns>
        [HttpPost("EndConversation")]
        public IActionResult EndConversation([FromBody] int conversationId)
        {
            _chatbot.EndConversation(conversationId);
            return Json(new { Success = true });
        }
    }
}
