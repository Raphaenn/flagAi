using App.Dto.CoupleAi;
using App.Interfaces.CoupleAi;
using App.Mappings;
using Domain.Entities;
using Domain.Interfaces;

namespace App.Services.CoupleAi;

public class ChatWithAiService : ICoupleAiService
{
    private readonly ICoupleAiRepositoryService _coupleAiRepositoryService;
    private readonly IConflictLabServiceRepository _conflictLabServiceRepository;

    public ChatWithAiService(ICoupleAiRepositoryService coupleAiRepositoryService, IConflictLabServiceRepository conflictLabServiceRepository)
    {
        _coupleAiRepositoryService = coupleAiRepositoryService;
        _conflictLabServiceRepository = conflictLabServiceRepository;
    }
    
    public async Task<CoupleMessageResponseDto> ChatWithAi(CoupleMessageRequestDto messageData, CancellationToken ct)
    {
        var parsed = new ChatContent(messageData.UserId, messageData.Message);
        
        ChatContent chatData = await _coupleAiRepositoryService.ProcessMessageAsync(parsed, ct);
        return ChatContentMap.ChatMessageToResponse(chatData);
    }

    public async Task<CoupleMessageResponseDto> ConflictAnalyzer(CoupleMessageRequestDto messageData, CancellationToken ct)
    {
        var parsed = new ChatContent(messageData.UserId, messageData.Message);
        ChatContent content = await _conflictLabServiceRepository.ProcessMessageAsync(parsed, ct);
        return ChatContentMap.ChatMessageToResponse(content);
    }
}