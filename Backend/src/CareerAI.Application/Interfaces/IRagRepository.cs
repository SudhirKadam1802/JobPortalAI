using CareerAI.Domain.Entities;

namespace CareerAI.Application.Interfaces;

public interface IRagRepository
{
    Task DeleteByResumeIdAsync(Guid resumeId);

    Task AddChunkAsync(RagDocumentChunk chunk);

    Task<List<RagDocumentChunk>> GetChunksByResumeIdAsync(
        Guid resumeId);

    Task SaveChangesAsync();
}