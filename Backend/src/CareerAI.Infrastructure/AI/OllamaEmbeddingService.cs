using CareerAI.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

namespace CareerAI.Infrastructure.AI;

public class OllamaEmbeddingService : IEmbeddingService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public OllamaEmbeddingService(
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<List<float>> GenerateEmbeddingAsync(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            throw new ArgumentException(
                "Text cannot be empty.");
        }

        var model = _configuration["Ollama:EmbeddingModel"]
            ?? throw new InvalidOperationException(
                "Ollama embedding model is missing.");

        var request = new
        {
            model = model,
            input = text
        };

        var response = await _httpClient.PostAsJsonAsync(
            "api/embed",
            request);

        response.EnsureSuccessStatusCode();

        var result =
            await response.Content
                .ReadFromJsonAsync<OllamaEmbeddingResponse>();

        if (result?.Embeddings is null ||
            result.Embeddings.Count == 0)
        {
            throw new InvalidOperationException(
                "Ollama did not return an embedding.");
        }

        return result.Embeddings[0];
    }

    private class OllamaEmbeddingResponse
    {
        public List<List<float>> Embeddings { get; set; }
            = new();
    }
}