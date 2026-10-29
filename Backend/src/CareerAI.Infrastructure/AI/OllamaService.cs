
using CareerAI.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace CareerAI.Infrastructure.AI;

public class OllamaService : IOllamaService
{
    private static readonly TimeSpan QuickGenerationTimeout =
        TimeSpan.FromSeconds(45);
    private static readonly TimeSpan LocalGenerationTimeout =
        TimeSpan.FromMinutes(2);

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
        return await GenerateAsync(
            prompt,
            CancellationToken.None,
            lowLatency: false);
    }

    public async Task<string> GenerateQuickAsync(
        string prompt,
        CancellationToken cancellationToken = default)
    {
        using var timeoutCts =
            CancellationTokenSource.CreateLinkedTokenSource(
                cancellationToken);
        var generationTimeout =
            string.IsNullOrWhiteSpace(
                _configuration["Gemini:ApiKey"])
                ? LocalGenerationTimeout
                : QuickGenerationTimeout;
        timeoutCts.CancelAfter(generationTimeout);

        try
        {
            return await GenerateAsync(
                prompt,
                timeoutCts.Token,
                lowLatency: true);
        }
        catch (OperationCanceledException exception)
            when (!cancellationToken.IsCancellationRequested)
        {
            throw new TimeoutException(
                $"AI generation exceeded the {generationTimeout.TotalSeconds:0}-second time limit.",
                exception);
        }
    }

    private async Task<string> GenerateAsync(
        string prompt,
        CancellationToken cancellationToken,
        bool lowLatency)
    {
        var apiKey = _configuration["Gemini:ApiKey"];

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            return await GenerateWithOllamaAsync(
                prompt,
                cancellationToken,
                lowLatency);
        }

        var primaryModel = _configuration["Gemini:Model"]
            ?? "gemini-2.5-flash";

        var fallbackModel = _configuration["Gemini:FallbackModel"];

        try
        {
            return await GenerateWithRetryAsync(
                prompt,
                apiKey,
                primaryModel,
                cancellationToken,
                lowLatency);
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
                prompt,
                apiKey,
                fallbackModel,
                cancellationToken,
                lowLatency);
        }
    }

    private async Task<string> GenerateWithOllamaAsync(
        string prompt,
        CancellationToken cancellationToken,
        bool lowLatency)
    {
        var model = _configuration["Ollama:GenerationModel"]
            ?? throw new InvalidOperationException(
                "Gemini API key and Ollama generation model are missing.");

        var requestBody = new Dictionary<string, object>
        {
            ["model"] = model,
            ["prompt"] = prompt,
            ["stream"] = false
        };

        if (lowLatency)
        {
            requestBody["think"] = false;
            requestBody["options"] = new
            {
                temperature = 0.2,
                num_predict = 500
            };
        }

        using var response = await _httpClient.PostAsJsonAsync(
            "api/generate",
            requestBody,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(
                cancellationToken);

            throw new HttpRequestException(
                $"Ollama returned {(int)response.StatusCode}: {error}",
                null,
                response.StatusCode);
        }

        var result =
            await response.Content.ReadFromJsonAsync<OllamaResponse>(
                cancellationToken: cancellationToken);

        if (string.IsNullOrWhiteSpace(result?.Response))
        {
            throw new InvalidOperationException(
                $"Ollama returned no text for model '{model}'.");
        }

        return result.Response;
    }

    private async Task<string> GenerateWithRetryAsync(
        string prompt,
        string apiKey,
        string model,
        CancellationToken cancellationToken,
        bool lowLatency)
    {
        for (var attempt = 1; attempt <= 2; attempt++)
        {
            try
            {
                return await GenerateOnceAsync(
                    prompt,
                    apiKey,
                    model,
                    cancellationToken,
                    lowLatency);
            }
            catch (HttpRequestException ex)
                when (attempt == 1 && IsTemporaryFailure(ex))
            {
                await Task.Delay(
                    TimeSpan.FromSeconds(1),
                    cancellationToken);
            }
        }

        throw new InvalidOperationException(
            $"Gemini generation failed for model '{model}'.");
    }

    private async Task<string> GenerateOnceAsync(
        string prompt,
        string apiKey,
        string model,
        CancellationToken cancellationToken,
        bool lowLatency)
    {
        var generationConfig = new Dictionary<string, object>
        {
            ["temperature"] = 0.2,
            ["maxOutputTokens"] = 500
        };

        if (lowLatency &&
            model.StartsWith(
                "gemini-2.5-flash",
                StringComparison.OrdinalIgnoreCase))
        {
            generationConfig["thinkingConfig"] = new
            {
                thinkingBudget = 0
            };
        }

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
            generationConfig
        };

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent");

        request.Headers.Add("x-goog-api-key", apiKey);
        request.Content = JsonContent.Create(requestBody);

        using var response = await _httpClient.SendAsync(
            request,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(
                cancellationToken);

            throw new HttpRequestException(
                $"Gemini API returned {(int)response.StatusCode} " +
                $"for model '{model}': {error}",
                null,
                response.StatusCode);
        }

        var result =
            await response.Content.ReadFromJsonAsync<GeminiResponse>(
                cancellationToken: cancellationToken);

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

    private sealed class OllamaResponse
    {
        public string? Response { get; set; }
    }
}
