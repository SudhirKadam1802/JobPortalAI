namespace CareerAI.Application.Features.RAG.DTOs;

public class RagIndexResponse
{
    public Guid ResumeId { get; set; }

    public int ChunksCreated { get; set; }

    public string Message { get; set; } = string.Empty;
}