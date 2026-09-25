using CareerAI.Application.Interfaces;
using CareerAI.Domain.Entities;
using CareerAI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CareerAI.Infrastructure.Repositories;

public class JobAlertRepository : IJobAlertRepository
{
    private readonly CareerAIDbContext _context;

    public JobAlertRepository(CareerAIDbContext context)
    {
        _context = context;
    }

    public async Task<JobAlert?> GetByIdAsync(Guid id)
    {
        return await _context.JobAlerts
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<JobAlert>> GetByCandidateIdAsync(
        Guid candidateId)
    {
        return await _context.JobAlerts
            .Where(x => x.CandidateId == candidateId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task AddAsync(JobAlert jobAlert)
    {
        await _context.JobAlerts.AddAsync(jobAlert);

        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(JobAlert jobAlert)
    {
        _context.JobAlerts.Update(jobAlert);

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(JobAlert jobAlert)
    {
        _context.JobAlerts.Remove(jobAlert);

        await _context.SaveChangesAsync();
    }
}