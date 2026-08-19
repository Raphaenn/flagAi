using Api.Middlewares;
using Api.Models;
using App.Interfaces;
using App.Interfaces.CoupleAi;
using App.Services.CoupleAi;
using App.UseCase;
using Infra.Services.AI;

namespace Api.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddAppInjections(this IServiceCollection services, IConfiguration config)
    {
        services.AddScoped<AnalyzeCompatibility>();
        services.AddScoped<ICoupleAiService, ChatWithAiService>();
        services.Auth(config);
        services.Configure<CoupleAiSettings>(
            config.GetSection(CoupleAiSettings.SectionName));
        
        services.AddHttpClient<ICompatibilityAiAnalyzer, CompatibilityAiClient>((sp, client) =>
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

        return services;
    }
}