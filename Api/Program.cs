using Api.Endpoints.ShipIt;
using Api.Middlewares;
using App.Abstractions;
using App.Services.Compatibility;
using Infra.Services.AI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<AnalyzeCompatibilityAppService>();
builder.Services.Auth(builder.Configuration);

builder.Services.AddHttpClient<ICompatibilityAiService, CompatibilityAiService>((sp, client) =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var baseUrl = config["Apis:AnthropicApi:BaseUrl"];
    if (string.IsNullOrWhiteSpace(baseUrl))
        throw new InvalidOperationException("Missing configuration: Apis:AnthropicApi:BaseUrl");

    var apiKey = config["Apis:AnthropicApi:ApiKey"];
    if (string.IsNullOrWhiteSpace(apiKey))
        throw new InvalidOperationException("Missing configuration: Apis:AnthropicApi:ApiKey");

    client.BaseAddress = new Uri(baseUrl);
    
    client.DefaultRequestHeaders.Add(
        "x-api-key",
        apiKey);
    client.DefaultRequestHeaders.Add(
        "anthropic-version",
        "2023-06-01");
});

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.UsePostBearerTokenGuard();

app.MapCreateShipIt();

app.Run();