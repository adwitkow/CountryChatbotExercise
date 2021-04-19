using System;
using System.Collections.Generic;

namespace CountryChatbotExercise.Core.Models
{
    internal class Conversation
    {
        public int Id { get; }
        public ICollection<Message> Messages { get; }

        public Conversation(int id)
        {
            Id = id;
            Messages = new List<Message>();
        }
    }
}
