
using CareerAI.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

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
        var apiKey = _configuration["Gemini:ApiKey"]
            ?? throw new InvalidOperationException(
                "Gemini API key is missing.");

        var primaryModel = _configuration["Gemini:Model"]
            ?? "gemini-2.5-flash";

        var fallbackModel = _configuration["Gemini:FallbackModel"];

        try
        {
            return await GenerateWithRetryAsync(
                prompt, apiKey, primaryModel);
        }
        catch (HttpRequestException ex)
            when (IsTemporaryFailure(ex) &&
                  !string.IsNullOrWhiteSpace(fallbackModel) &&
                  !string.Equals(
                      primaryModel,
                      fallbackModel,
                      StringComparison.OrdinalIgnoreCase))
        {
            // The primary model is temporarily unavailable.
            // Try the configured fallback model.
            return await GenerateWithRetryAsync(
                prompt, apiKey, fallbackModel);
        }
    }

    private async Task<string> GenerateWithRetryAsync(
        string prompt,
        string apiKey,
        string model)
    {
        for (var attempt = 1; attempt <= 2; attempt++)
        {
            try
            {
                return await GenerateOnceAsync(
                    prompt, apiKey, model);
            }
            catch (HttpRequestException ex)
                when (attempt == 1 && IsTemporaryFailure(ex))
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }

        throw new InvalidOperationException(
            $"Gemini generation failed for model '{model}'.");
    }

    private async Task<string> GenerateOnceAsync(
        string prompt,
        string apiKey,
        string model)
    {
        var requestBody = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new { text = prompt }
                    }
                }
            },
            generationConfig = new
            {
                temperature = 0.2,
                maxOutputTokens = 500
            }
        };

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent");

        request.Headers.Add("x-goog-api-key", apiKey);
        request.Content = JsonContent.Create(requestBody);

        using var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();

            throw new HttpRequestException(
                $"Gemini API returned {(int)response.StatusCode} " +
                $"for model '{model}': {error}",
                null,
                response.StatusCode);
        }

        var result =
            await response.Content.ReadFromJsonAsync<GeminiResponse>();

        var text = result?.Candidates?
            .FirstOrDefault()?
            .Content?
            .Parts?
            .FirstOrDefault()?
            .Text;

        if (string.IsNullOrWhiteSpace(text))
        {
            throw new InvalidOperationException(
                $"Gemini returned no text for model '{model}'.");
        }

        return text;
    }

    private static bool IsTemporaryFailure(
        HttpRequestException exception)
    {
        return exception.StatusCode is
            HttpStatusCode.TooManyRequests or
            HttpStatusCode.InternalServerError or
            HttpStatusCode.BadGateway or
            HttpStatusCode.ServiceUnavailable or
            HttpStatusCode.GatewayTimeout;
    }

    private sealed class GeminiResponse
    {
        public List<GeminiCandidate>? Candidates { get; set; }
    }

    private sealed class GeminiCandidate
    {
        public GeminiContent? Content { get; set; }
    }

    private sealed class GeminiContent
    {
        public List<GeminiPart>? Parts { get; set; }
    }

    private sealed class GeminiPart
    {
        public string? Text { get; set; }
    }
}
