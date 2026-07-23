using Api.Models.ShipIt;
using App.Services.Compatibility;

namespace Api.Endpoints.ShipIt;

public static class CreateShipItEndpoint
{
    public static IEndpointRouteBuilder MapCreateShipIt(
        this IEndpointRouteBuilder app)
    {
        app.MapGet("/shipits/ai-health", (IConfiguration configuration) =>
        {
            var baseUrl = configuration["Apis:AnthropicApi:BaseUrl"];
            var apiKey = configuration["Apis:AnthropicApi:ApiKey"];

            var baseUrlValid = Uri.TryCreate(baseUrl, UriKind.Absolute, out _);
            var apiKeyConfigured = !string.IsNullOrWhiteSpace(apiKey);

            var promptPath = ResolvePromptPath();
            var promptExists = promptPath is not null && File.Exists(promptPath);
            var promptReadable = false;
            var promptSizeBytes = 0L;

            if (promptExists)
            {
                try
                {
                    var content = File.ReadAllText(promptPath!);
                    promptReadable = !string.IsNullOrWhiteSpace(content);
                    promptSizeBytes = new FileInfo(promptPath!).Length;
                }
                catch
                {
                    promptReadable = false;
                }
            }

            var healthy = baseUrlValid && apiKeyConfigured && promptExists && promptReadable;

            var response = new
            {
                status = healthy ? "healthy" : "unhealthy",
                checks = new
                {
                    baseUrl = new
                    {
                        configured = !string.IsNullOrWhiteSpace(baseUrl),
                        valid = baseUrlValid
                    },
                    apiKey = new
                    {
                        configured = apiKeyConfigured,
                        length = apiKey?.Length ?? 0
                    },
                    prompt = new
                    {
                        exists = promptExists,
                        readable = promptReadable,
                        path = promptPath,
                        sizeBytes = promptSizeBytes
                    }
                }
            };

            return healthy
                ? Results.Ok(response)
                : Results.Problem(
                    title: "AI integration health check failed",
                    detail: "One or more IA integration checks are invalid.",
                    statusCode: StatusCodes.Status503ServiceUnavailable,
                    extensions: new Dictionary<string, object?>
                    {
                        ["checks"] = response.checks
                    });
        });

        app.MapPost("/shipit/compatibility", async (
            CreateShipItRequest request,
            AnalyzeCompatibilityAppService appService,
            CancellationToken cancellationToken) =>
        {
            if (string.IsNullOrWhiteSpace(request.Description))
                return Results.BadRequest("Description is required.");

            if (string.IsNullOrWhiteSpace(request.PersonA.Name) || string.IsNullOrWhiteSpace(request.PersonA.Email))
                return Results.BadRequest("PersonA name and email are required.");

            if (string.IsNullOrWhiteSpace(request.PersonB.Name) || string.IsNullOrWhiteSpace(request.PersonB.Email))
                return Results.BadRequest("PersonB name and email are required.");

            var result = await appService.AnalyzeAsync(
                request.Description,
                request.PersonA.Name,
                request.PersonA.Email,
                request.PersonB.Name,
                request.PersonB.Email,
                cancellationToken);

            return Results.Ok(new
            {
                PersonA = new
                {
                    Name = request.PersonA.Name,
                    Email = request.PersonA.Email,
                    result.PersonA.Normalized
                },
                PersonB = new
                {
                    Name = request.PersonB.Name,
                    Email = request.PersonB.Email,
                    result.PersonB.Normalized
                },
                result.Summary,
                result.Strengths,
                result.PossibleConflicts,
                result.MissingInfo,
                result.Score,
                result.Compatibility
            });
        }).RequireAuthorization();

        return app;
    }

    private static string? ResolvePromptPath()
    {
        var current = Directory.GetCurrentDirectory();

        var candidates = new[]
        {
            Path.Combine(current, "prompt.txt"),
            Path.Combine(current, "..", "Infra", "prompt.txt"),
            Path.Combine(current, "Infra", "prompt.txt")
        };

        foreach (var candidate in candidates)
        {
            var fullPath = Path.GetFullPath(candidate);
            if (File.Exists(fullPath))
                return fullPath;
        }

        return null;
    }
}