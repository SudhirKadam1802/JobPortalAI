
using CareerAI.Application.Interfaces;
using CareerAI.Domain.Entities;
using CareerAI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CareerAI.Infrastructure.Repositories;

public class AIChatRepository : IAIChatRepository
{
    private readonly CareerAIDbContext _context;

    public AIChatRepository(CareerAIDbContext context)
    {
        _context = context;
    }

    // ========================================
    // Get One Conversation
    // ========================================

    public async Task<AIConversation?> GetConversationAsync(
        Guid userId,
        Guid conversationId)
    {
        return await _context.AIConversations
            .FirstOrDefaultAsync(x =>
                x.Id == conversationId &&
                x.UserId == userId);
    }

    // ========================================
    // Get All User Conversations
    // ========================================

    public async Task<List<AIConversation>>
        GetConversationsByUserIdAsync(
            Guid userId)
    {
        return await _context.AIConversations
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt)
            .ToListAsync();
    }

    // ========================================
    // Get Conversation Messages
    // ========================================

    public async Task<List<AIMessage>>
        GetMessagesAsync(
            Guid conversationId)
    {
        return await _context.AIMessages
            .Where(x =>
                x.ConversationId == conversationId)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync();
    }

    // ========================================
    // Add Conversation
    // ========================================

    public async Task AddConversationAsync(
        AIConversation conversation)
    {
        await _context.AIConversations
            .AddAsync(conversation);
    }

    // ========================================
    // Add Message
    // ========================================

    public async Task AddMessageAsync(
        AIMessage message)
    {
        await _context.AIMessages
            .AddAsync(message);
    }

    // ========================================
    // Delete Conversation
    // ========================================

    public async Task<bool> DeleteConversationAsync(
        Guid userId,
        Guid conversationId)
    {
        // Find the conversation belonging to this user.
        var conversation = await _context.AIConversations
            .FirstOrDefaultAsync(x =>
                x.Id == conversationId &&
                x.UserId == userId);

        // Conversation not found or not owned by user.
        if (conversation is null)
        {
            return false;
        }

        // Find all messages belonging to this conversation.
        var messages = await _context.AIMessages
            .Where(x =>
                x.ConversationId == conversationId)
            .ToListAsync();

        // Delete the conversation's messages first.
        if (messages.Count > 0)
        {
            _context.AIMessages.RemoveRange(messages);
        }

        // Delete the conversation.
        _context.AIConversations.Remove(conversation);

        return true;
    }

    // ========================================
    // Save Changes
    // ========================================

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}