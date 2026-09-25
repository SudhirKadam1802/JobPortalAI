using CareerAI.Application.Features.RecruiterDashboard.DTOs;
using CareerAI.Application.Interfaces;

namespace CareerAI.Application.Features.RecruiterDashboard.Services;

public class RecruiterDashboardService
    : IRecruiterDashboardService
{
    private readonly IRecruiterDashboardRepository
        _recruiterDashboardRepository;

    public RecruiterDashboardService(
        IRecruiterDashboardRepository recruiterDashboardRepository)
    {
        _recruiterDashboardRepository =
            recruiterDashboardRepository;
    }

    public async Task<RecruiterDashboardResponse>
        GetDashboardAsync(Guid recruiterId)
    {
        return await _recruiterDashboardRepository
            .GetDashboardAsync(recruiterId);
    }
}