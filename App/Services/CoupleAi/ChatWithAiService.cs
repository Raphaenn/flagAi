using App.Dto.CoupleAi;
using App.Interfaces.CoupleAi;
using App.Mappings;
using Domain.Entities;
using Domain.Interfaces;

namespace App.Services.CoupleAi;

public class ChatWithAiService : ICoupleAiService
{
    private readonly ICoupleAiRepositoryService _coupleAiRepositoryService;

    public ChatWithAiService(ICoupleAiRepositoryService coupleAiRepositoryService)
    {
        _coupleAiRepositoryService = coupleAiRepositoryService;
    }
    
    public async Task<CoupleMessageResponseDto> ChatWithAi(CoupleMessageRequestDto messageData, CancellationToken ct)
    {
        var parsed = new ChatContent(messageData.UserId, messageData.Message);
        
        ChatContent chatData = await _coupleAiRepositoryService.ProcessMessageAsync(parsed, ct);
        return ChatContentMap.ChatMessageToResponse(chatData);
    }
}