using CareerAI.Application.Features.JobAlerts.DTOs;
using CareerAI.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CareerAI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Candidate")]
public class JobAlertController : ControllerBase
{
    private readonly IJobAlertService _jobAlertService;

    public JobAlertController(
        IJobAlertService jobAlertService)
    {
        _jobAlertService = jobAlertService;
    }


    // =========================================================
    // CREATE JOB ALERT
    // =========================================================

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateJobAlertRequest request)
    {
        var userId = GetUserId();

        var jobAlert =
            await _jobAlertService.CreateAsync(
                userId,
                request);

        return Ok(jobAlert);
    }


    // =========================================================
    // GET MY JOB ALERTS
    // =========================================================

    [HttpGet("my")]
    public async Task<IActionResult> GetMyAlerts()
    {
        var userId = GetUserId();

        var jobAlerts =
            await _jobAlertService
                .GetMyAlertsAsync(userId);

        return Ok(jobAlerts);
    }


    // =========================================================
    // GET JOB ALERT BY ID
    // =========================================================

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        Guid id)
    {
        var userId = GetUserId();

        var jobAlert =
            await _jobAlertService
                .GetByIdAsync(userId, id);

        if (jobAlert is null)
        {
            return NotFound(
                new
                {
                    message = "Job alert not found."
                });
        }

        return Ok(jobAlert);
    }


    // =========================================================
    // UPDATE JOB ALERT
    // =========================================================

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateJobAlertRequest request)
    {
        var userId = GetUserId();

        var jobAlert =
            await _jobAlertService
                .UpdateAsync(
                    userId,
                    id,
                    request);

        if (jobAlert is null)
        {
            return NotFound(
                new
                {
                    message = "Job alert not found."
                });
        }

        return Ok(jobAlert);
    }


    // =========================================================
    // DELETE JOB ALERT
    // =========================================================

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id)
    {
        var userId = GetUserId();

        var deleted =
            await _jobAlertService
                .DeleteAsync(
                    userId,
                    id);

        if (!deleted)
        {
            return NotFound(
                new
                {
                    message = "Job alert not found."
                });
        }

        return Ok(
            new
            {
                message = "Job alert deleted successfully."
            });
    }


    // =========================================================
    // GET USER ID
    // =========================================================

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