namespace App.Dto.CoupleAi;

public class CoupleMessageRequestDto
{
    public string UserId { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public List<MessageDto>? ConversationHistory { get; set; } // opcional historic
}