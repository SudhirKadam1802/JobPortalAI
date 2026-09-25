using CareerAI.Application.Features.JobAlerts.DTOs;

namespace CareerAI.Application.Interfaces;

public interface IJobAlertService
{
    Task<JobAlertResponse> CreateAsync(
        Guid userId,
        CreateJobAlertRequest request);

    Task<List<JobAlertResponse>> GetMyAlertsAsync(
        Guid userId);

    Task<JobAlertResponse?> GetByIdAsync(
        Guid userId,
        Guid id);

    Task<JobAlertResponse?> UpdateAsync(
        Guid userId,
        Guid id,
        UpdateJobAlertRequest request);

    Task<bool> DeleteAsync(
        Guid userId,
        Guid id);
}