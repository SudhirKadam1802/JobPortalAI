using CareerAI.Application.Features.CandidateProfile.DTOs;
using CareerAI.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CareerAI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Candidate")]
public class CandidateProfileController : ControllerBase
{
    private readonly ICandidateProfileService
        _candidateProfileService;

    public CandidateProfileController(
        ICandidateProfileService candidateProfileService)
    {
        _candidateProfileService =
            candidateProfileService;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMyProfile()
    {
        var userId = GetUserId();

        var profile =
            await _candidateProfileService
                .GetMyProfileAsync(userId);

        if (profile is null)
        {
            return NotFound(
                new
                {
                    message = "Candidate profile not found."
                });
        }

        return Ok(profile);
    }

    [HttpPut("me")]
    public async Task<IActionResult> UpdateMyProfile(
        UpdateCandidateProfileRequest request)
    {
        var userId = GetUserId();

        var profile =
            await _candidateProfileService
                .UpdateMyProfileAsync(
                    userId,
                    request);

        if (profile is null)
        {
            return NotFound(
                new
                {
                    message = "Candidate profile not found."
                });
        }

        return Ok(profile);
    }

    private Guid GetUserId()
    {
        var userId =
            User.FindFirstValue(
                ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue(
                ClaimTypes.Name);

        if (!Guid.TryParse(
                userId,
                out var parsedUserId))
        {
            throw new UnauthorizedAccessException(
                "Invalid user identity.");
        }

        return parsedUserId;
    }
}