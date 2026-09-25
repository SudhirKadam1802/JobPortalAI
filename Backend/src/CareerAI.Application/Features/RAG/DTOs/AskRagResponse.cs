namespace CareerAI.Application.Features.RAG.DTOs;

public class AskRagResponse
{
    public Guid ResumeId { get; set; }

    public string Question { get; set; } = string.Empty;

    public string Answer { get; set; } = string.Empty;

    public int ChunksUsed { get; set; }
}