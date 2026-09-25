namespace CareerAI.Domain.Entities;

public class RagDocumentChunk
{
    public Guid Id { get; set; }

    public Guid ResumeId { get; set; }

    public string ChunkText { get; set; } = string.Empty;

    public string Embedding { get; set; } = string.Empty;

    public int ChunkIndex { get; set; }

    public DateTime CreatedAt { get; set; }

    public Resume Resume { get; set; } = null!;
}