namespace CountryChatbotExercise.Core.Models
{
    public enum MessageStatus
    {
        Begin, // Currently unused since we rely on ConversationId being null
        Converse,
        End
    }
}