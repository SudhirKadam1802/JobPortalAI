using CareerAI.Domain.Entities;

namespace CareerAI.Application.Interfaces;

public interface IAIChatRepository
{
    // Get One Conversation
    Task<AIConversation?> GetConversationAsync(
        Guid userId,
        Guid conversationId);

    // Get All User Conversations
    Task<List<AIConversation>> GetConversationsByUserIdAsync(
        Guid userId);

    // Get Conversation Messages
    Task<List<AIMessage>> GetMessagesAsync(
        Guid conversationId);

    // Add Conversation
    Task AddConversationAsync(
        AIConversation conversation);

    // Add Message
    Task AddMessageAsync(
        AIMessage message);

    // Delete Conversation
    Task<bool> DeleteConversationAsync(
        Guid userId,
        Guid conversationId);

    // Save Changes
    Task SaveChangesAsync();
}