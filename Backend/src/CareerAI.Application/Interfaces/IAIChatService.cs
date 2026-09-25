using CareerAI.Application.Features.AIChat.DTOs;

namespace CareerAI.Application.Interfaces;

public interface IAIChatService
{
    Task<AIConversationResponse> CreateConversationAsync(
        Guid userId,
        string? title);

    Task<AIMessageResponse> SendMessageAsync(
        Guid userId,
        Guid conversationId,
        string message);

    Task<List<AIConversationResponse>> GetMyConversationsAsync(
        Guid userId);

    Task<AIConversationResponse?> GetConversationAsync(
        Guid userId,
        Guid conversationId);

    // Delete a specific conversation
    Task<bool> DeleteConversationAsync(
        Guid userId,
        Guid conversationId);
}