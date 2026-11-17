using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Domain.Entities;
using Domain.Interfaces;
using Infra.Contracts.CoupleAi;
using Infra.Helpers;
using Infra.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infra.Services.CoupleAi;

public class ConflictLabServiceRepository : IConflictLabServiceRepository
{
    private readonly HttpClient _httpClient;
    private readonly ApiSettings _settings;
    private readonly ILogger<ConflictLabServiceRepository> _logger;
    private readonly ConversationHistoryService _historyService;
    private readonly string _systemPrompt;
    
    public ConflictLabServiceRepository(
        HttpClient httpClient, 
        IOptions<ApiSettings> options,
        ILogger<ConflictLabServiceRepository> logger, 
        ConversationHistoryService historyService
        )
    {
        _httpClient = httpClient;
        _settings = options.Value;
        _logger = logger;
        _historyService = historyService;
    }

    public async Task<ChatContent> ProcessMessageAsync(ChatContent chat, CancellationToken cancellationToken = default)
    {
        var baseDir = AppContext.BaseDirectory;
        string path = Path.Combine(baseDir, "prompts");
        MarkdownReader content = new MarkdownReader(path, "conflictLab.txt");
        string prompt = await content.ReadAsync(cancellationToken);
        
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

        try
        {
            if (chat is null)
            {
                throw new ArgumentNullException(nameof(chat));
            }

            var request = new
            {
                model = _settings.Model,
                max_tokens = _settings.MaxTokens,
                system = new[]
                {
                    new
                    {
                        type = "text",
                        text = prompt,
                        cache_control = new
                        {
                            type = "ephemeral"
                        }
                    }
                },
                messages
            };
            
            var stopwatch = Stopwatch.StartNew();
            using var response = await _httpClient.PostAsJsonAsync("v1/messages", request, _jsonOptions, cancellationToken);
            
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<AnthropicResponse>(cancellationToken: cancellationToken);
            
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
            
            var output = new ChatContent(userId: chat.UserId, message: chat.Message)
            {
                ConversationHistory = history,
                Status = true,
                Error = string.Empty
            };
            
            output.Message = assistantMessage;

            return output;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };
}