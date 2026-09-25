using CareerAI.Application.Interfaces;
using CareerAI.Domain.Entities;
using CareerAI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CareerAI.Infrastructure.Repositories;

public class JobRecommendationRepository : IJobRecommendationRepository
{
    private readonly CareerAIDbContext _context;

    public JobRecommendationRepository(CareerAIDbContext context)
    {
        _context = context;
    }

    public async Task<List<JobRecommendation>> GetByCandidateIdAsync(
        Guid candidateId)
    {
        return await _context.JobRecommendations
            .Include(x => x.Job)
            .Where(x => x.CandidateId == candidateId)
            .OrderByDescending(x => x.MatchScore)
            .ToListAsync();
    }

    public async Task<JobRecommendation?> GetByCandidateAndJobAsync(
        Guid candidateId,
        Guid jobId)
    {
        return await _context.JobRecommendations
            .FirstOrDefaultAsync(x =>
                x.CandidateId == candidateId &&
                x.JobId == jobId);
    }

    public async Task AddAsync(
        JobRecommendation recommendation)
    {
        await _context.JobRecommendations
            .AddAsync(recommendation);
    }

    public void Update(
        JobRecommendation recommendation)
    {
        _context.JobRecommendations
            .Update(recommendation);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}