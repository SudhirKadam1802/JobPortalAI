using CareerAI.Application.Features.Jobs.DTOs;
using CareerAI.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CareerAI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobController : ControllerBase
{
    private readonly IJobService _jobService;

    public JobController(IJobService jobService)
    {
        _jobService = jobService;
    }

    // ========================================
    // Get All Jobs
    // GET: api/job
    // ========================================

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var jobs = await _jobService.GetAllAsync();

        return Ok(jobs);
    }

    // ========================================
    // Get Job By Id
    // GET: api/job/{id}
    // ========================================

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var job = await _jobService.GetByIdAsync(id);

        if (job is null)
        {
            return NotFound(new
            {
                message = "Job not found."
            });
        }

        return Ok(job);
    }

    // ========================================
    // Create Job
    // POST: api/job
    // ========================================

    [Authorize(Roles = "Recruiter")]
    [HttpPost]
    public async Task<IActionResult> Create(
        CreateJobRequest request)
    {
        var userId = GetUserId();

        var job = await _jobService.CreateAsync(
            userId,
            request);

        return CreatedAtAction(
            nameof(GetById),
            new { id = job.Id },
            job);
    }

    // ========================================
    // Update Job
    // PUT: api/job/{id}
    // ========================================

    [Authorize(Roles = "Recruiter")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateJobRequest request)
    {
        var userId = GetUserId();

        var job = await _jobService.UpdateAsync(
            userId,
            id,
            request);

        if (job is null)
        {
            return NotFound(new
            {
                message = "Job not found."
            });
        }

        return Ok(job);
    }

    // ========================================
    // Delete Job
    // DELETE: api/job/{id}
    // ========================================

    [Authorize(Roles = "Recruiter")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = GetUserId();

        var deleted = await _jobService.DeleteAsync(
            userId,
            id);

        if (!deleted)
        {
            return NotFound(new
            {
                message = "Job not found."
            });
        }

        return Ok(new
        {
            message = "Job deleted successfully."
        });
    }

    // ========================================
    // Get Logged-in User ID
    // ========================================

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