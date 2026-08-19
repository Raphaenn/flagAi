using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Domain.Entities;
using Domain.Interfaces;
using Infra.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infra.Services.CoupleAi;
    
public class CoupleAiRepositoryService : ICoupleAiRepositoryService
{
    private readonly HttpClient _httpClient;
    private readonly ApiSettings _settings;
    private readonly ILogger<CoupleAiRepositoryService> _logger;
    private readonly ConversationHistoryService _historyService;

    public CoupleAiRepositoryService(
        HttpClient httpClient,
        IOptions<ApiSettings> options,
        ILogger<CoupleAiRepositoryService> logger,
        ConversationHistoryService historyService)
    {
        _httpClient = httpClient;
        _settings = options.Value;
        _logger = logger;
        _historyService = historyService;
    }
    
    public async Task<ChatContent> ProcessMessageAsync(ChatContent chat, CancellationToken ct)
    {
        try
        {
            if (chat is null)
            {
                throw new ArgumentNullException(nameof(chat));
            }
            
            List<Message> history = chat.ConversationHistory is { Count: > 0 }
                ? new List<Message>(chat.ConversationHistory)
                : _historyService.GetHistory(chat.UserId);
            
            var messages = history
                .TakeLast(10)
                .Select(message => new
                {
                    role = message.Role,
                    content = message.Content
                })
                .ToList();
            
            messages.Add(new
            {
                role = "user",
                content = chat.Message
            });

            // Monta o payload da API
            string? prompt = ReadFromFile();
            if (prompt == null)
            {
                return new ChatContent(userId: chat.UserId, message: chat.Message)
                {
                    Status = false,
                    Error = "Prompt error"
                };
            }
            
            var request = new
            {
                model = _settings.Model,
                max_tokens = _settings.MaxTokens,
                messages
            };

            if (!Uri.TryCreate(_settings.BaseUrl, UriKind.Absolute, out var baseUri))
            {
                throw new InvalidOperationException("ApiSettings.BaseUrl deve ser uma URI absoluta válida.");
            }

            var endpoint = new Uri(baseUri, "v1/messages");
            var response = await _httpClient.PostAsJsonAsync(endpoint, request, ct);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<AnthropicResponse>(cancellationToken: ct);
            var assistantMessage = result?
                .Content?
                .FirstOrDefault()?
                .Text;
            
            if (string.IsNullOrWhiteSpace(assistantMessage))
            {
                return new ChatContent(userId: chat.UserId, message: chat.Message)
                {
                    Status = false,
                    Error = "The AI returned an empty response."
                };
            }
            
            history.Add(new Message
            {
                Role = "user",
                Content = chat.Message
            });
            _historyService.SaveHistory(chat.UserId, history);
            
            _logger.LogDebug(
                "AI response for user {UserId}: {Response}",
                chat.UserId,
                assistantMessage);
            var output = new ChatContent(userId: chat.UserId, message: chat.Message)
            {
                ConversationHistory = history,
                Status = true,
                Error = string.Empty
            };
            output.Message = assistantMessage;

            return output;

        }
        catch (Exception ex)
        {
            // _logger.LogError(ex, "Erro ao processar mensagem (User: {UserId})", request.UserId);
            return new ChatContent(userId: chat.UserId, message: chat.Message)
            {
                Status = false,
                Error = $"Erro ao processar: {ex.Message}"
            };
        }
    }

    public bool IsConfigured()
    {
        return !string.IsNullOrWhiteSpace(_settings.AnthropicApiKey);
    }
    
    private sealed record AnthropicResponse(IReadOnlyList<AnthropicContentPart>? Content);

    private sealed record AnthropicContentPart(string Type, string? Text);

    private string? ReadFromFile()
    {
        var baseDir = AppContext.BaseDirectory;
        var currentDir = Directory.GetCurrentDirectory();

        var candidates = new[]
        {
            Path.Combine(baseDir, "prompts", "cupid.txt"),
            Path.Combine(currentDir, "prompts", "cupid.txt"),
            Path.Combine(currentDir, "cupid.txt"),
            Path.Combine(currentDir, "..", "Infra", "prompts", "cupid.txt")
        };

        foreach (var candidate in candidates)
        {
            var fullPath = Path.GetFullPath(candidate);
            if (File.Exists(fullPath))
                return File.ReadAllText(fullPath);
        }

        return null;
    }
}
