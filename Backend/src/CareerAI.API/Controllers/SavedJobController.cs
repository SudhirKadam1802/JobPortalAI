using CareerAI.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CareerAI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Candidate")]
public class SavedJobController : ControllerBase
{
    private readonly ISavedJobService _savedJobService;

    public SavedJobController(
        ISavedJobService savedJobService)
    {
        _savedJobService = savedJobService;
    }


    // =========================================================
    // SAVE JOB
    // =========================================================

    [HttpPost("{jobId:guid}")]
    public async Task<IActionResult> SaveJob(
        Guid jobId)
    {
        var userId = GetUserId();

        var savedJob =
            await _savedJobService.SaveAsync(
                userId,
                jobId);

        return Ok(savedJob);
    }


    // =========================================================
    // GET MY SAVED JOBS
    // =========================================================

    [HttpGet("my")]
    public async Task<IActionResult> GetMySavedJobs()
    {
        var userId = GetUserId();

        var savedJobs =
            await _savedJobService
                .GetMySavedJobsAsync(userId);

        return Ok(savedJobs);
    }


    // =========================================================
    // CHECK WHETHER JOB IS SAVED
    // =========================================================

    [HttpGet("check/{jobId:guid}")]
    public async Task<IActionResult> CheckSavedJob(
        Guid jobId)
    {
        var userId = GetUserId();

        var isSaved =
            await _savedJobService
                .IsSavedAsync(
                    userId,
                    jobId);

        return Ok(new
        {
            jobId,
            isSaved
        });
    }


    // =========================================================
    // REMOVE SAVED JOB
    // =========================================================

    [HttpDelete("{jobId:guid}")]
    public async Task<IActionResult> RemoveSavedJob(
        Guid jobId)
    {
        var userId = GetUserId();

        var removed =
            await _savedJobService
                .RemoveAsync(
                    userId,
                    jobId);

        if (!removed)
        {
            return NotFound(new
            {
                message = "Saved job not found."
            });
        }

        return Ok(new
        {
            message = "Job removed from saved jobs."
        });
    }


    // =========================================================
    // GET USER ID FROM JWT
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