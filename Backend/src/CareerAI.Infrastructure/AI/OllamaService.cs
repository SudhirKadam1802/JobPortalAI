using CareerAI.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

namespace CareerAI.Infrastructure.AI;

public class OllamaService : IOllamaService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public OllamaService(
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<string> GenerateAsync(string prompt)
    {
        var model = _configuration["Ollama:GenerationModel"]
            ?? throw new InvalidOperationException(
                "Ollama generation model is missing.");

        var request = new
        {
            model = model,
            prompt = prompt,
            stream = false,
            think = false,
            options = new
            {
                temperature = 0.2,
                num_predict = 500
            }
        };

        var response = await _httpClient.PostAsJsonAsync(
            "api/generate",
            request);

        response.EnsureSuccessStatusCode();

        var result =
            await response.Content.ReadFromJsonAsync<OllamaResponse>();

        return result?.Response ?? string.Empty;
    }

    private class OllamaResponse
    {
        public string Response { get; set; } = string.Empty;
    }
}