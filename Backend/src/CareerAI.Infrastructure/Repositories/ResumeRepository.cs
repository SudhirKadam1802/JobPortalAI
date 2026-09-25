using CareerAI.Application.Interfaces;
using CareerAI.Domain.Entities;
using CareerAI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CareerAI.Infrastructure.Repositories;

public class ResumeRepository : IResumeRepository
{
    private readonly CareerAIDbContext _context;

    public ResumeRepository(
        CareerAIDbContext context)
    {
        _context = context;
    }

    public async Task<List<Resume>> GetByCandidateIdAsync(
        Guid candidateId)
    {
        return await _context.Resumes
            .Where(x => x.CandidateId == candidateId)
            .OrderByDescending(x => x.UploadedAt)
            .ToListAsync();
    }

    public async Task<Resume?> GetByIdAsync(
        Guid id)
    {
        return await _context.Resumes
            .Include(x => x.Candidate)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task AddAsync(
        Resume resume)
    {
        await _context.Resumes.AddAsync(resume);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}