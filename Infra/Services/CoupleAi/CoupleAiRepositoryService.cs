using System.Diagnostics;
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
    private readonly string _systemPrompt;

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
        _systemPrompt = ReadFromFile()
                        ?? throw new InvalidOperationException("AI prompt not found.");
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
                .TakeLast(4)
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

            var request = new
            {
                model = _settings.Model,
                max_tokens = _settings.MaxTokens,
                system = new[]
                {
                    new
                        {
                        type = "text",
                        text = _systemPrompt,
                        cache_control = new
                        {
                            type = "ephemeral"
                        }
                    }
                },
                messages
            };
            
            var stopwatch = Stopwatch.StartNew();

            using var response = await _httpClient.PostAsJsonAsync(
                "v1/messages",
                request,
                _jsonOptions,
                ct);
            
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<AnthropicResponse>(cancellationToken: ct);
            
            stopwatch.Stop();
            
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
            
            history.Add(new Message
            {
                Role = "assistant",
                Content = assistantMessage
            });
            
            _historyService.SaveHistory(chat.UserId, history);
            
            var usage = result?.Usage;
            
            _logger.LogInformation(
                """
                Claude API metrics -
                InputTokens: {InputTokens},
                OutputTokens: {OutputTokens},
                CacheCreated: {CacheCreated},
                CacheRead: {CacheRead},
                DurationMs: {DurationMs}
                """,
                usage?.InputTokens ?? 0,
                usage?.OutputTokens ?? 0,
                usage?.CacheCreationInputTokens ?? 0,
                usage?.CacheReadInputTokens ?? 0,
                stopwatch.ElapsedMilliseconds);
            
            // _logger.LogDebug(
            //     "AI response for user {UserId}: {Response}",
            //     chat.UserId,
            //     assistantMessage);
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
    
    private sealed record AnthropicResponse(IReadOnlyList<AnthropicContentPart>? Content, AnthropicUsage? Usage);

    private sealed record AnthropicContentPart(string Type, string? Text);
    
    private sealed record AnthropicUsage(
        [property: JsonPropertyName("input_tokens")]
        int InputTokens,

        [property: JsonPropertyName("output_tokens")]
        int OutputTokens,
        
        [property: JsonPropertyName("cache_creation_input_tokens")]
        int CacheCreationInputTokens,

        [property: JsonPropertyName("cache_read_input_tokens")]
        int CacheReadInputTokens
        );

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
    
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };
}
