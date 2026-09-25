using CareerAI.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CareerAI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Candidate")]
public class ResumeController : ControllerBase
{
    private readonly IResumeService _resumeService;
    private readonly IResumeAnalysisService _resumeAnalysisService;

    public ResumeController(
        IResumeService resumeService,
        IResumeAnalysisService resumeAnalysisService)
    {
        _resumeService = resumeService;
        _resumeAnalysisService = resumeAnalysisService;
    }

    // ==========================================
    // Upload Resume
    // ==========================================

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        var userId = GetUserId();

        var resume = await _resumeService
            .UploadAsync(userId, file);

        return Ok(resume);
    }

    // ==========================================
    // Get My Resumes
    // ==========================================

    [HttpGet("my")]
    public async Task<IActionResult> GetMyResumes()
    {
        var userId = GetUserId();

        var resumes = await _resumeService
            .GetMyResumesAsync(userId);

        return Ok(resumes);
    }

    // ==========================================
    // Get Resume By ID
    // ==========================================

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var userId = GetUserId();

        var resume = await _resumeService
            .GetByIdAsync(userId, id);

        if (resume is null)
        {
            return NotFound(new
            {
                message = "Resume not found."
            });
        }

        return Ok(resume);
    }

    // ==========================================
    // Extract Resume Text
    // ==========================================

    [HttpGet("{id:guid}/text")]
    public async Task<IActionResult> ExtractText(Guid id)
    {
        var userId = GetUserId();

        var text = await _resumeService
            .ExtractTextAsync(userId, id);

        return Ok(new
        {
            resumeId = id,
            text = text
        });
    }

    // ==========================================
    // AI Resume Analysis
    // ==========================================

    [HttpPost("{id:guid}/analyze")]
    public async Task<IActionResult> Analyze(Guid id)
    {
        var userId = GetUserId();

        var analysis = await _resumeAnalysisService
            .AnalyzeAsync(userId, id);

        return Ok(analysis);
    }

    // ==========================================
    // Get Existing Resume Analysis
    // ==========================================

    [HttpGet("{id:guid}/analysis")]
    public async Task<IActionResult> GetAnalysis(Guid id)
    {
        var userId = GetUserId();

        var analysis = await _resumeAnalysisService
            .GetAnalysisAsync(userId, id);

        if (analysis is null)
        {
            return NotFound(new
            {
                message = "Resume analysis not found."
            });
        }

        return Ok(analysis);
    }

    // ==========================================
    // Get User ID From JWT
    // ==========================================

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