namespace CareerAI.Application.Features.RAG.DTOs;

public class AskRagRequest
{
    public Guid ResumeId { get; set; }

    public string Question { get; set; } = string.Empty;
}