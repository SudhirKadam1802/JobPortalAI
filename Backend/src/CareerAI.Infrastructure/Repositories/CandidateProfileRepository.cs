using CareerAI.Application.Interfaces;
using CareerAI.Domain.Entities;
using CareerAI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CareerAI.Infrastructure.Repositories;

public class CandidateProfileRepository
    : ICandidateProfileRepository
{
    private readonly CareerAIDbContext _context;

    public CandidateProfileRepository(
        CareerAIDbContext context)
    {
        _context = context;
    }

    public async Task<Candidate?> GetByUserIdAsync(
        Guid userId)
    {
        return await _context.Candidates
            .Include(x => x.User)
            .FirstOrDefaultAsync(
                x => x.UserId == userId);
    }

    public async Task UpdateAsync(
        Candidate candidate)
    {
        _context.Candidates.Update(candidate);

        if (candidate.User is not null)
        {
            _context.Users.Update(candidate.User);
        }

        await _context.SaveChangesAsync();
    }
}