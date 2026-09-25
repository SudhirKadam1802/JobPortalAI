using CareerAI.Application.Interfaces;
using CareerAI.Domain.Entities;
using CareerAI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CareerAI.Infrastructure.Repositories;

public class RecruiterRepository : IRecruiterRepository
{
    private readonly CareerAIDbContext _context;

    public RecruiterRepository(CareerAIDbContext context)
    {
        _context = context;
    }

    public async Task<Recruiter?> GetByUserIdAsync(Guid userId)
    {
        return await _context.Recruiters
            .FirstOrDefaultAsync(x => x.UserId == userId);
    }

    public async Task<Recruiter?> GetByIdAsync(Guid id)
    {
        return await _context.Recruiters
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task AddAsync(Recruiter recruiter)
    {
        await _context.Recruiters.AddAsync(recruiter);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}