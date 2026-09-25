using CareerAI.Application.Interfaces;
using CareerAI.Domain.Entities;
using CareerAI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CareerAI.Infrastructure.Repositories;

public class ResumeAnalysisRepository : IResumeAnalysisRepository
{
    private readonly CareerAIDbContext _context;

    public ResumeAnalysisRepository(CareerAIDbContext context)
    {
        _context = context;
    }

    public async Task<ResumeAnalysis?> GetByResumeIdAsync(
        Guid resumeId)
    {
        return await _context.ResumeAnalyses
            .FirstOrDefaultAsync(x => x.ResumeId == resumeId);
    }

    public async Task AddAsync(
        ResumeAnalysis analysis)
    {
        await _context.ResumeAnalyses.AddAsync(analysis);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}