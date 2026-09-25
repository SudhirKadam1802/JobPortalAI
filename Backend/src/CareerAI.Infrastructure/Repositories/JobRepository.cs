using CareerAI.Application.Interfaces;
using CareerAI.Domain.Entities;
using CareerAI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CareerAI.Infrastructure.Repositories;

public class JobRepository : IJobRepository
{
    private readonly CareerAIDbContext _context;

    public JobRepository(CareerAIDbContext context)
    {
        _context = context;
    }

    public async Task<List<Job>> GetAllAsync()
    {
        return await _context.Jobs
            .Include(j => j.Recruiter)
            .Include(j => j.JobSkills)
                .ThenInclude(js => js.Skill)
            .ToListAsync();
    }

    public async Task<Job?> GetByIdAsync(Guid id)
    {
        return await _context.Jobs
            .Include(j => j.Recruiter)
            .Include(j => j.JobSkills)
                .ThenInclude(js => js.Skill)
            .FirstOrDefaultAsync(j => j.Id == id);
    }

    public async Task AddAsync(Job job)
    {
        await _context.Jobs.AddAsync(job);
    }

    public void Update(Job job)
    {
        _context.Jobs.Update(job);
    }

    public void Delete(Job job)
    {
        _context.Jobs.Remove(job);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}