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

    // ========================================
    // Get All Jobs
    // ========================================

    public async Task<List<Job>> GetAllAsync()
    {
        return await _context.Jobs
            .Include(x => x.Recruiter)
            .Include(x => x.JobSkills)
            .ThenInclude(x => x.Skill)
            .ToListAsync();
    }

    // ========================================
    // Get Job By Id
    // ========================================

    public async Task<Job?> GetByIdAsync(Guid id)
    {
        return await _context.Jobs
            .Include(x => x.Recruiter)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    // ========================================
    // Add Job
    // ========================================

    public async Task AddAsync(Job job)
    {
        await _context.Jobs.AddAsync(job);
    }

    // ========================================
    // Update Job
    // ========================================

    public void Update(Job job)
    {
        _context.Jobs.Update(job);
    }

    // ========================================
    // Delete Job
    // ========================================

    public void Delete(Job job)
    {
        _context.Jobs.Remove(job);
    }

    // ========================================
    // Save Changes
    // ========================================

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}