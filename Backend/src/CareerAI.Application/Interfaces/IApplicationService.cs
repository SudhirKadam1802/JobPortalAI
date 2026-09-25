using CareerAI.Application.Features.Applications.DTOs;

namespace CareerAI.Application.Interfaces;

public interface IApplicationService
{
    Task<ApplicationResponse> ApplyAsync(
        Guid userId,
        CreateApplicationRequest request);

    Task<List<ApplicationResponse>> GetMyApplicationsAsync(
        Guid userId);

    Task<List<ApplicationResponse>> GetJobApplicationsAsync(
        Guid userId,
        Guid jobId);

    Task<ApplicationResponse?> GetByIdAsync(
        Guid userId,
        Guid applicationId);

    Task<ApplicationResponse?> UpdateStatusAsync(
        Guid userId,
        Guid applicationId,
        UpdateApplicationStatusRequest request);
}