using CareerAI.Application.Features.AIInterview.DTOs;
using CareerAI.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CareerAI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Candidate")]
public class AIInterviewController : ControllerBase
{
    private readonly IAIInterviewService _interviewService;

    public AIInterviewController(
        IAIInterviewService interviewService)
    {
        _interviewService = interviewService;
    }

    [HttpPost("start")]
    public async Task<IActionResult> StartInterview(
        [FromBody] StartInterviewRequest request)
    {
        var userId = GetUserId();

        var response =
            await _interviewService
                .StartInterviewAsync(
                    userId,
                    request);

        return Ok(response);
    }

    [HttpPost("{interviewId:guid}/answer")]
    public async Task<IActionResult> SubmitAnswer(
        Guid interviewId,
        [FromBody] SubmitAnswerRequest request)
    {
        var userId = GetUserId();

        var response =
            await _interviewService
                .SubmitAnswerAsync(
                    userId,
                    interviewId,
                    request);

        return Ok(response);
    }

    [HttpGet("{interviewId:guid}/result")]
    public async Task<IActionResult> GetInterviewResult(
        Guid interviewId)
    {
        var userId = GetUserId();

        var result =
            await _interviewService
                .GetInterviewResultAsync(
                    userId,
                    interviewId);

        if (result is null)
        {
            return NotFound(new
            {
                message = "Interview not found."
            });
        }

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