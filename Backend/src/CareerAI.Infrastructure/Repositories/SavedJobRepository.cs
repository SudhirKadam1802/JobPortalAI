using CareerAI.Application.Interfaces;
using CareerAI.Domain.Entities;
using CareerAI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CareerAI.Infrastructure.Repositories;

public class SavedJobRepository : ISavedJobRepository
{
    private readonly CareerAIDbContext _context;

    public SavedJobRepository(
        CareerAIDbContext context)
    {
        _context = context;
    }


    // =========================================================
    // GET SAVED JOB
    // =========================================================

    public async Task<SavedJob?> GetAsync(
        Guid candidateId,
        Guid jobId)
    {
        return await _context.SavedJobs
            .Include(x => x.Job)
            .FirstOrDefaultAsync(x =>
                x.CandidateId == candidateId &&
                x.JobId == jobId);
    }


    // =========================================================
    // GET ALL SAVED JOBS FOR CANDIDATE
    // =========================================================

    public async Task<List<SavedJob>> GetByCandidateIdAsync(
        Guid candidateId)
    {
        return await _context.SavedJobs
            .Include(x => x.Job)
            .Where(x => x.CandidateId == candidateId)
            .OrderByDescending(x => x.SavedAt)
            .ToListAsync();
    }


    // =========================================================
    // ADD SAVED JOB
    // =========================================================

    public async Task AddAsync(
        SavedJob savedJob)
    {
        await _context.SavedJobs.AddAsync(savedJob);

        await _context.SaveChangesAsync();
    }


    // =========================================================
    // DELETE SAVED JOB
    // =========================================================

    public async Task DeleteAsync(
        SavedJob savedJob)
    {
        _context.SavedJobs.Remove(savedJob);

        await _context.SaveChangesAsync();
    }
}