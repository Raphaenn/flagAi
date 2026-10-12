using Api.Models;
using App.Dto.CoupleAi;
using App.Interfaces.CoupleAi;
using Microsoft.Extensions.Options;

namespace Api.Endpoints.CoupleAi;

public static class CoupleAiEndpoint
{
    public record struct RequestMessage(string UserId, string Message);
    
    public static IEndpointRouteBuilder MapCreateCoupleAi(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/couple-ai").WithTags("CoupleAi");

        group.MapPost("/version", GetVersion);
        group.MapGet("/health", HealthCheck);
        group.MapPost("/message", CreateCouple);

        return group;
    }

    private static IResult GetVersion()
    {
        return Results.Ok(new
        {
            version = "1.0.2",
            timestamp = DateTime.UtcNow
        });
    }
    
    private static IResult HealthCheck(IOptions<CoupleAiSettings> options)
    {
        CoupleAiSettings settings = options.Value;
        bool isConfigured =
            !string.IsNullOrWhiteSpace(settings.AnthropicApiKey);

            
        return Results.Ok(new
        {
            status = "healthy",
            version = 4.1,
            timestamp = DateTime.UtcNow,
            configured = isConfigured,
            message = isConfigured 
                ? "API está configurada e pronta para uso" 
                : "AVISO: API key não configurada"
        });
    }

    private async static Task<IResult> CreateCouple(CoupleMessageRequestDto req, ICoupleAiService coupleAiService, 
        CancellationToken cancellationToken)
    {
        var response = await coupleAiService.ChatWithAi(req, cancellationToken);
        return Results.Ok(response);
    }
    
    // todo -> Create stream endpoint to better responses
}