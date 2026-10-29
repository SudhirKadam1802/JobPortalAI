namespace CareerAI.Application.Interfaces;

public interface IOllamaService
{
    Task<string> GenerateAsync(string prompt);

    Task<string> GenerateQuickAsync(
        string prompt,
        CancellationToken cancellationToken = default);
}