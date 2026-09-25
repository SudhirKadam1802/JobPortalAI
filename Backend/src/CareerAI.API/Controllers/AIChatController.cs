using CareerAI.Application.Features.AIChat.DTOs;
using CareerAI.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CareerAI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Candidate")]
public class AIChatController : ControllerBase
{
    private readonly IAIChatService _chatService;

    public AIChatController(
        IAIChatService chatService)
    {
        _chatService = chatService;
    }

    // ========================================
    // Create New Conversation
    // ========================================

    [HttpPost("conversations")]
    public async Task<IActionResult> CreateConversation(
        [FromBody] CreateConversationRequest request)
    {
        var userId = GetUserId();

        var conversation =
            await _chatService
                .CreateConversationAsync(
                    userId,
                    request.Title);

        return Ok(conversation);
    }

    // ========================================
    // Send Message
    // ========================================

    [HttpPost("conversations/{conversationId:guid}/messages")]
    public async Task<IActionResult> SendMessage(
        Guid conversationId,
        [FromBody] SendMessageRequest request)
    {
        var userId = GetUserId();

        var response =
            await _chatService
                .SendMessageAsync(
                    userId,
                    conversationId,
                    request.Message);

        return Ok(response);
    }

    // ========================================
    // Get My Conversations
    // ========================================

    [HttpGet("conversations")]
    public async Task<IActionResult> GetMyConversations()
    {
        var userId = GetUserId();

        var conversations =
            await _chatService
                .GetMyConversationsAsync(
                    userId);

        return Ok(conversations);
    }

    // ========================================
    // Get One Conversation
    // ========================================

    [HttpGet("conversations/{conversationId:guid}")]
    public async Task<IActionResult> GetConversation(
        Guid conversationId)
    {
        var userId = GetUserId();

        var conversation =
            await _chatService
                .GetConversationAsync(
                    userId,
                    conversationId);

        if (conversation is null)
        {
            return NotFound(new
            {
                message = "Conversation not found."
            });
        }

        return Ok(conversation);
    }

    // ========================================
    // Get Logged-in User ID
    // ========================================

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

    // ========================================
    // Delete Conversation
    // ========================================

    [HttpDelete("conversations/{conversationId:guid}")]
    public async Task<IActionResult> DeleteConversation(
        Guid conversationId)
    {
        var userId = GetUserId();

        var deleted =
            await _chatService.DeleteConversationAsync(
                userId,
                conversationId);

        if (!deleted)
        {
            return NotFound(new
            {
                message = "Conversation not found."
            });
        }

        return Ok(new
        {
            message = "Conversation deleted successfully."
        });
    }
}