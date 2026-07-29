namespace Infra.Models
{
    /// <summary>
    /// API SETTINGS
    /// </summary>
    public class ApiSettings
    {
        public string BaseUrl { get; set; } = string.Empty;
        public string AnthropicApiKey { get; set; } = string.Empty;
        public int MaxTokens { get; set; } = 1024;
        public string Model { get; set; } = "claude-haiku-4-5-20251001"; // Modelo leve e rápido
    }
}