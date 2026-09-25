using CareerAI.Domain.Entities;

namespace CareerAI.Application.Interfaces;

public interface ISavedJobRepository
{
    Task<SavedJob?> GetAsync(
        Guid candidateId,
        Guid jobId);

    Task<List<SavedJob>> GetByCandidateIdAsync(
        Guid candidateId);

    Task AddAsync(
        SavedJob savedJob);

    Task DeleteAsync(
        SavedJob savedJob);
}