namespace CareerAI.Application.Interfaces;

public interface IEmbeddingService
{
    Task<List<float>> GenerateEmbeddingAsync(string text);
}