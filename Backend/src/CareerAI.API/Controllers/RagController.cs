using CareerAI.Application.Features.RAG.DTOs;
using CareerAI.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CareerAI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Candidate")]
public class RagController : ControllerBase
{
    private readonly IRagService _ragService;

    public RagController(IRagService ragService)
    {
        _ragService = ragService;
    }

    [HttpPost("index/{resumeId:guid}")]
    public async Task<IActionResult> IndexResume(
        Guid resumeId)
    {
        var userId = GetUserId();

        var result =
            await _ragService.IndexResumeAsync(
                userId,
                resumeId);

        return Ok(result);
    }

    [HttpPost("ask")]
    public async Task<IActionResult> Ask(
        [FromBody] AskRagRequest request)
    {
        var userId = GetUserId();

        var result =
            await _ragService.AskAsync(
                userId,
                request);

        return Ok(result);
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