using CareerAI.Application.Interfaces;
using CareerAI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using ApplicationEntity = CareerAI.Domain.Entities.Application;

namespace CareerAI.Infrastructure.Repositories;

public class ApplicationRepository : IApplicationRepository
{
    private readonly CareerAIDbContext _context;

    public ApplicationRepository(CareerAIDbContext context)
    {
        _context = context;
    }

    public async Task<List<ApplicationEntity>> GetByCandidateIdAsync(
        Guid candidateId)
    {
        return await _context.Applications
            .Include(x => x.Job)
            .Where(x => x.CandidateId == candidateId)
            .OrderByDescending(x => x.AppliedAt)
            .ToListAsync();
    }

    public async Task<List<ApplicationEntity>> GetByJobIdAsync(
        Guid jobId)
    {
        return await _context.Applications
            .Include(x => x.Candidate)
            .ThenInclude(x => x.User)
            .Where(x => x.JobId == jobId)
            .OrderByDescending(x => x.AppliedAt)
            .ToListAsync();
    }

    public async Task<ApplicationEntity?> GetByIdAsync(Guid id)
    {
        return await _context.Applications
            .Include(x => x.Candidate)
            .ThenInclude(x => x.User)
            .Include(x => x.Job)
            .ThenInclude(x => x.Recruiter)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<bool> ExistsAsync(
        Guid candidateId,
        Guid jobId)
    {
        return await _context.Applications
            .AnyAsync(x =>
                x.CandidateId == candidateId &&
                x.JobId == jobId);
    }

    public async Task AddAsync(ApplicationEntity application)
    {
        await _context.Applications.AddAsync(application);
    }

    public void Update(ApplicationEntity application)
    {
        _context.Applications.Update(application);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}