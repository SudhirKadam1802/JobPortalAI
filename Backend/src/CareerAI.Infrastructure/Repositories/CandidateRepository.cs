
using CareerAI.Application.Interfaces;
using CareerAI.Domain.Entities;
using CareerAI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CareerAI.Infrastructure.Repositories;

public class CandidateRepository : ICandidateRepository
{
    private readonly CareerAIDbContext _context;

    public CandidateRepository(CareerAIDbContext context)
    {
        _context = context;
    }

    public async Task<Candidate?> GetByUserIdAsync(Guid userId)
    {
        return await _context.Candidates
            .Include(x => x.User)
            .Include(x => x.CandidateSkills)
            .ThenInclude(x => x.Skill)
            .Include(x => x.Experiences)
            .Include(x => x.Educations)
            .FirstOrDefaultAsync(x => x.UserId == userId);
    }

    public async Task AddAsync(Candidate candidate)
    {
        await _context.Candidates.AddAsync(candidate);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}