using Domain.Entities;

namespace Domain.Interfaces;

public interface IConflictLabServiceRepository
{
    Task<ChatContent> ProcessMessageAsync(ChatContent chat, CancellationToken cancellationToken = default);
}