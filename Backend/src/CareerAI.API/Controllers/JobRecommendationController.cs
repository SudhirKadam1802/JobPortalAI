using CareerAI.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CareerAI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Candidate")]
public class JobRecommendationController : ControllerBase
{
    private readonly IJobRecommendationService _recommendationService;

    public JobRecommendationController(
        IJobRecommendationService recommendationService)
    {
        _recommendationService = recommendationService;
    }

    [HttpPost("generate")]
    public async Task<IActionResult> GenerateRecommendations()
    {
        var userId = GetUserId();

        var recommendations =
            await _recommendationService
                .GenerateRecommendationsAsync(userId);

        return Ok(recommendations);
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyRecommendations()
    {
        var userId = GetUserId();

        var recommendations =
            await _recommendationService
                .GetMyRecommendationsAsync(userId);

        return Ok(recommendations);
    }

    private Guid GetUserId()
    {
        var userId =
            User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue(ClaimTypes.Name);

        if (!Guid.TryParse(userId, out var parsedUserId))
        {
            throw new UnauthorizedAccessException(
                "Invalid user identity.");
        }

        return parsedUserId;
    }
}