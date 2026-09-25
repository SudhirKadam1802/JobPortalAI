namespace CareerAI.Application.Interfaces;

public interface IResumeTextExtractor
{
    Task<string> ExtractTextAsync(string filePath);
}