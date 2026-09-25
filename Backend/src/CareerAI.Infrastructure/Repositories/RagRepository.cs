using CareerAI.Application.Interfaces;
using CareerAI.Domain.Entities;
using CareerAI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CareerAI.Infrastructure.Repositories;

public class RagRepository : IRagRepository
{
    private readonly CareerAIDbContext _context;

    public RagRepository(CareerAIDbContext context)
    {
        _context = context;
    }

    public async Task DeleteByResumeIdAsync(Guid resumeId)
    {
        var chunks = await _context.RagDocumentChunks
            .Where(x => x.ResumeId == resumeId)
            .ToListAsync();

        if (chunks.Count > 0)
        {
            _context.RagDocumentChunks.RemoveRange(chunks);
        }
    }

    public async Task AddChunkAsync(RagDocumentChunk chunk)
    {
        await _context.RagDocumentChunks.AddAsync(chunk);
    }

    public async Task<List<RagDocumentChunk>>
        GetChunksByResumeIdAsync(Guid resumeId)
    {
        return await _context.RagDocumentChunks
            .Where(x => x.ResumeId == resumeId)
            .OrderBy(x => x.ChunkIndex)
            .ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}