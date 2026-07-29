using Domain.Entities;

namespace Domain.Interfaces;

public interface ICoupleAiRepositoryService
{
    Task<ChatContent> ProcessMessageAsync(ChatContent chat, CancellationToken cancellationToken = default);
}