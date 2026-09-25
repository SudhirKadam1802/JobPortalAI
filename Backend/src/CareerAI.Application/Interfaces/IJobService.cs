using CareerAI.Application.Features.Jobs.DTOs;

namespace CareerAI.Application.Interfaces;

public interface IJobService
{
    Task<JobResponse> CreateAsync(
        Guid userId,
        CreateJobRequest request);

    Task<List<JobResponse>> GetAllAsync();

    Task<JobResponse?> GetByIdAsync(Guid id);

    Task<JobResponse?> UpdateAsync(
        Guid userId,
        Guid id,
        UpdateJobRequest request);

    Task<bool> DeleteAsync(
        Guid userId,
        Guid id);
}