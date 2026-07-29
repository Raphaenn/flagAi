using System.Collections.Concurrent;
using Domain.Entities;

namespace Infra.Services.CoupleAi;

public class ConversationHistoryService
{
    private readonly ConcurrentDictionary<string, List<Message>> _histories = new();
    
    public List<Message> GetHistory(string userId)
        => _histories.GetOrAdd(userId, _ => new List<Message>());

    public void SaveHistory(string userId, List<Message> history)
        => _histories[userId] = history;

    public void ClearHistory(string userId)
        => _histories.TryRemove(userId, out _);
}