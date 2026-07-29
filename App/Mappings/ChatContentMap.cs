using App.Dto.CoupleAi;
using Domain.Entities;

namespace App.Mappings;

public static class ChatContentMap
{
    public static CoupleMessageResponseDto ChatMessageToResponse(ChatContent content)
    {
        List<MessageDto> msgs = new List<MessageDto>();
        if (content?.ConversationHistory?.Count > 0)
        {
            foreach (var c in content.ConversationHistory)
            {
                msgs.Add(new(c.Role, c.Content, DateTime.UtcNow));
            }
        }

        return new CoupleMessageResponseDto
        {
            Success = content.Status,
            Response = content.Message,
            Error = content.Error,
            ConversationHistory = msgs
        };
    }
}