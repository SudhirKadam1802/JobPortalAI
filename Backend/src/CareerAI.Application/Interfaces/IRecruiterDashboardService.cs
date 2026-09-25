namespace CareerAI.Application.Interfaces;

public interface IRecruiterDashboardService
{
    Task<Features.RecruiterDashboard.DTOs.RecruiterDashboardResponse>
        GetDashboardAsync(Guid recruiterId);
}