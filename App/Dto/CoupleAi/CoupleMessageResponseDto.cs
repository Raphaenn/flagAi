namespace App.Dto.CoupleAi;

public class CoupleMessageResponseDto
{
    public bool Success { get; set; }
    public string Response { get; set; } = string.Empty;
    public string? Error { get; set; }
    public List<MessageDto> ConversationHistory { get; set; } = new();
}