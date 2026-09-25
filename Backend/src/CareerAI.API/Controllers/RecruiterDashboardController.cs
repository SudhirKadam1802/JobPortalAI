using CareerAI.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CareerAI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Recruiter")]
public class RecruiterDashboardController : ControllerBase
{
    private readonly IRecruiterDashboardService
        _recruiterDashboardService;

    public RecruiterDashboardController(
        IRecruiterDashboardService recruiterDashboardService)
    {
        _recruiterDashboardService =
            recruiterDashboardService;
    }

    [HttpGet]
    public async Task<IActionResult> GetDashboard()
    {
        var userId =
            User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue(ClaimTypes.Name);

        if (!Guid.TryParse(userId, out var parsedUserId))
        {
            throw new UnauthorizedAccessException(
                "Invalid user identity.");
        }

        var dashboard =
            await _recruiterDashboardService
                .GetDashboardAsync(parsedUserId);

        return Ok(dashboard);
    }
}