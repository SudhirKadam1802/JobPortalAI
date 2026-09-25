using CareerAI.Application.Features.SavedJobs.DTOs;

namespace CareerAI.Application.Interfaces;

public interface ISavedJobService
{
    Task<SavedJobResponse> SaveAsync(
        Guid userId,
        Guid jobId);

    Task<List<SavedJobResponse>> GetMySavedJobsAsync(
        Guid userId);

    Task<bool> IsSavedAsync(
        Guid userId,
        Guid jobId);

    Task<bool> RemoveAsync(
        Guid userId,
        Guid jobId);
}