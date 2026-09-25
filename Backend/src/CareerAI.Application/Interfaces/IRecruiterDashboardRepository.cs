using CareerAI.Application.Features.RecruiterDashboard.DTOs;

namespace CareerAI.Application.Interfaces;

public interface IRecruiterDashboardRepository
{
    Task<RecruiterDashboardResponse> GetDashboardAsync(
        Guid recruiterId);
}