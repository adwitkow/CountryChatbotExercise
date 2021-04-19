using System;

namespace CountryChatbotExercise.Core.Models
{
    public class Message
    {
        public int? ConversationId { get; set; }
        public MessageStatus? Status { get; set; }
        public MessageType MessageType { get; set; }
        public string Body { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
