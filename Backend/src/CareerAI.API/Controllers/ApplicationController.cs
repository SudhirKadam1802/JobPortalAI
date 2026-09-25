using CareerAI.Application.Features.Applications.DTOs;
using CareerAI.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CareerAI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ApplicationController : ControllerBase
{
    private readonly IApplicationService _applicationService;

    public ApplicationController(
        IApplicationService applicationService)
    {
        _applicationService = applicationService;
    }

    // ==========================================
    // Candidate: Apply for a Job
    // ==========================================

    [Authorize(Roles = "Candidate")]
    [HttpPost]
    public async Task<IActionResult> Apply(
        CreateApplicationRequest request)
    {
        var userId = GetUserId();

        var application =
            await _applicationService.ApplyAsync(
                userId,
                request);

        return Ok(application);
    }

    // ==========================================
    // Candidate: Get My Applications
    // ==========================================

    [Authorize(Roles = "Candidate")]
    [HttpGet("my")]
    public async Task<IActionResult> GetMyApplications()
    {
        var userId = GetUserId();

        var applications =
            await _applicationService
                .GetMyApplicationsAsync(userId);

        return Ok(applications);
    }

    // ==========================================
    // Recruiter: Get Applications for a Job
    // ==========================================

    [Authorize(Roles = "Recruiter")]
    [HttpGet("job/{jobId:guid}")]
    public async Task<IActionResult> GetJobApplications(
        Guid jobId)
    {
        var userId = GetUserId();

        var applications =
            await _applicationService
                .GetJobApplicationsAsync(
                    userId,
                    jobId);

        return Ok(applications);
    }

    // ==========================================
    // Candidate / Recruiter: Get Application
    // ==========================================

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        Guid id)
    {
        var userId = GetUserId();

        var application =
            await _applicationService
                .GetByIdAsync(
                    userId,
                    id);

        if (application is null)
        {
            return NotFound(new
            {
                message = "Application not found."
            });
        }

        return Ok(application);
    }

    // ==========================================
    // Recruiter: Update Application Status
    // ==========================================

    [Authorize(Roles = "Recruiter")]
    [HttpPut("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(
        Guid id,
        UpdateApplicationStatusRequest request)
    {
        var userId = GetUserId();

        var application =
            await _applicationService
                .UpdateStatusAsync(
                    userId,
                    id,
                    request);

        if (application is null)
        {
            return NotFound(new
            {
                message = "Application not found."
            });
        }

        return Ok(application);
    }

    // ==========================================
    // Get User ID from JWT
    // ==========================================

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