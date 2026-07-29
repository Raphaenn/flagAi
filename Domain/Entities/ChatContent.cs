namespace Domain.Entities;

public class ChatContent
{
    public string UserId { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public List<Message>? ConversationHistory { get; set; }
    public bool Status { get; set; }
    public string Error { get; set; } = string.Empty;

    public ChatContent(string userId, string message)
    {
        if (message.Length < 20)
        {
            throw new Exception("To short message");
        }

        this.UserId = userId;
        this.Message = message;
    }
}

public class Message
{
    public string Role { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime Timestamp  { get; set; }
}