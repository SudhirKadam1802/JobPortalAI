using CareerAI.Application.Interfaces;
using System.Net.Http.Json;

namespace CareerAI.Infrastructure.AI;

public class OllamaEmbeddingService : IEmbeddingService
{
    private readonly HttpClient _httpClient;

    public OllamaEmbeddingService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<float>> GenerateEmbeddingAsync(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            throw new ArgumentException(
                "Text cannot be empty.");
        }

        var request = new
        {
            model = "embeddinggemma",
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