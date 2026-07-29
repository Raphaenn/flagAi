using Domain.Interfaces;
using Infra.Models;
using Infra.Services.CoupleAi;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Infra;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration config)
    {
        /*
         * Get ApiSettings from app settings
         * O .Bind(...) faz o mapeamento automático: cada chave do JSON (BaseUrl, AnthropicApiKey) é jogada na propriedade de mesmo nome na classe ApiSettings. É basicamente um JsonSerializer fazendo o de-para nome-por-nome.
         */
        services.AddOptions<ApiSettings>()
            .Bind(config.GetSection("ApiSettings"))
            .Validate(s => !string.IsNullOrWhiteSpace(s.BaseUrl), "ApiSettings.BaseUrl não pode ser vazio")
            .Validate(s => !string.IsNullOrWhiteSpace(s.AnthropicApiKey), "ApiSettings.AnthropicApiKey não pode ser vazio")
            .ValidateOnStart();
        
        // ValidateOnStart - Por que isso importa na prática: sem isso, se você esquecer de configurar AnthropicApiKey no appsettings.json, sua API vai subir normalmente, aceitar requisições, e só vai explodir quando o primeiro usuário chamar o endpoint que precisa da IA — um erro que só aparece em produção, na hora errada. Com ValidateOnStart(), a aplicação nem sobe, e você vê o erro no log de inicialização, no ambiente de deploy, antes de qualquer usuário ser afetado. Isso é o princípio de fail-fast.

        services.AddSingleton<ConversationHistoryService>();
        services.AddHttpClient<ICoupleAiRepositoryService, CoupleAiRepositoryService>((sp, client) =>
        {
            var settings = sp.GetRequiredService<IOptions<ApiSettings>>().Value;

            client.BaseAddress = new Uri(settings.BaseUrl);
            client.DefaultRequestHeaders.Add("x-api-key", settings.AnthropicApiKey);
            client.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");
        });

        return services;
    }
}