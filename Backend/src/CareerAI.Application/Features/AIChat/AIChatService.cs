
using CareerAI.Application.Features.AIChat.DTOs;
using CareerAI.Application.Interfaces;
using CareerAI.Domain.Entities;
using CareerAI.Domain.Enums;

namespace CareerAI.Application.Features.AIChat;

public class AIChatService : IAIChatService
{
    private readonly IAIChatRepository _chatRepository;
    private readonly IOllamaService _ollamaService;

    public AIChatService(
        IAIChatRepository chatRepository,
        IOllamaService ollamaService)
    {
        _chatRepository = chatRepository;
        _ollamaService = ollamaService;
    }

    // ========================================
    // Create New Conversation
    // ========================================

    public async Task<AIConversationResponse>
        CreateConversationAsync(
            Guid userId,
            string? title)
    {
        var conversation =
            new AIConversation
            {
                Id = Guid.NewGuid(),

                UserId = userId,

                Title = string.IsNullOrWhiteSpace(title)
                    ? "New Career Chat"
                    : title.Trim(),

                CreatedAt = DateTime.UtcNow,

                UpdatedAt = DateTime.UtcNow
            };

        await _chatRepository
            .AddConversationAsync(conversation);

        await _chatRepository
            .SaveChangesAsync();

        return new AIConversationResponse
        {
            Id = conversation.Id,

            Title = conversation.Title,

            CreatedAt = conversation.CreatedAt,

            UpdatedAt = conversation.UpdatedAt,

            Messages = new List<AIMessageResponse>()
        };
    }

    // ========================================
    // Send Message
    // ========================================

    public async Task<AIMessageResponse>
        SendMessageAsync(
            Guid userId,
            Guid conversationId,
            string message)
    {
        // ========================================
        // Validate Message
        // ========================================

        if (string.IsNullOrWhiteSpace(message))
        {
            throw new ArgumentException(
                "Message cannot be empty.");
        }

        // ========================================
        // Verify Conversation Ownership
        // ========================================

        var conversation =
            await _chatRepository
                .GetConversationAsync(
                    userId,
                    conversationId);

        if (conversation is null)
        {
            throw new KeyNotFoundException(
                "Conversation not found.");
        }

        // ========================================
        // Get Previous Messages
        // ========================================

        var previousMessages =
            await _chatRepository
                .GetMessagesAsync(
                    conversationId);

        // ========================================
        // Save User Message
        // ========================================

        var userMessage =
            new AIMessage
            {
                Id = Guid.NewGuid(),

                ConversationId =
                    conversationId,

                Role =
                    AIMessageRole.User,

                Content =
                    message.Trim(),

                CreatedAt =
                    DateTime.UtcNow
            };

        await _chatRepository
            .AddMessageAsync(userMessage);

        // ========================================
        // Build Conversation Prompt
        // ========================================

        var prompt =
            BuildPrompt(
                previousMessages,
                message.Trim());

        // ========================================
        // Call Ollama / Qwen3
        // ========================================

        var aiResponse =
            await _ollamaService
                .GenerateAsync(prompt);

        if (string.IsNullOrWhiteSpace(aiResponse))
        {
            throw new Exception(
                "AI returned an empty response.");
        }

        // ========================================
        // Save AI Message
        // ========================================

        var assistantMessage =
            new AIMessage
            {
                Id = Guid.NewGuid(),

                ConversationId =
                    conversationId,

                Role =
                    AIMessageRole.Assistant,

                Content =
                    aiResponse.Trim(),

                CreatedAt =
                    DateTime.UtcNow
            };

        await _chatRepository
            .AddMessageAsync(
                assistantMessage);

        // ========================================
        // Update Conversation
        // ========================================

        conversation.UpdatedAt =
            DateTime.UtcNow;

        // ========================================
        // Save Everything
        // ========================================

        await _chatRepository
            .SaveChangesAsync();

        // ========================================
        // Return AI Response
        // ========================================

        return MapMessageToResponse(
            assistantMessage);
    }

    // ========================================
    // Get My Conversations
    // ========================================

    public async Task<List<AIConversationResponse>>
        GetMyConversationsAsync(
            Guid userId)
    {
        var conversations =
            await _chatRepository
                .GetConversationsByUserIdAsync(
                    userId);

        var responses =
            new List<AIConversationResponse>();

        foreach (var conversation in conversations)
        {
            var messages =
                await _chatRepository
                    .GetMessagesAsync(
                        conversation.Id);

            responses.Add(
                new AIConversationResponse
                {
                    Id =
                        conversation.Id,

                    Title =
                        conversation.Title,

                    CreatedAt =
                        conversation.CreatedAt,

                    UpdatedAt =
                        conversation.UpdatedAt,

                    Messages =
                        messages
                            .Select(
                                MapMessageToResponse)
                            .ToList()
                });
        }

        return responses;
    }

    // ========================================
    // Get One Conversation
    // ========================================

    public async Task<AIConversationResponse?>
        GetConversationAsync(
            Guid userId,
            Guid conversationId)
    {
        var conversation =
            await _chatRepository
                .GetConversationAsync(
                    userId,
                    conversationId);

        if (conversation is null)
        {
            return null;
        }

        var messages =
            await _chatRepository
                .GetMessagesAsync(
                    conversationId);

        return new AIConversationResponse
        {
            Id =
                conversation.Id,

            Title =
                conversation.Title,

            CreatedAt =
                conversation.CreatedAt,

            UpdatedAt =
                conversation.UpdatedAt,

            Messages =
                messages
                    .Select(
                        MapMessageToResponse)
                    .ToList()
        };
    }

    // ========================================
    // Delete Conversation
    // ========================================

    public async Task<bool> DeleteConversationAsync(
        Guid userId,
        Guid conversationId)
    {
        // Delete only the conversation owned by this user.
        var deleted =
            await _chatRepository
                .DeleteConversationAsync(
                    userId,
                    conversationId);

        if (!deleted)
        {
            return false;
        }

        // Save the deletion to the database.
        await _chatRepository
            .SaveChangesAsync();

        return true;
    }

    // ========================================
    // Build AI Prompt
    // ========================================

    private static string BuildPrompt(
        List<AIMessage> previousMessages,
        string currentMessage)
    {
        var conversationHistory =
            previousMessages.Count == 0
                ? "No previous conversation."
                : string.Join(
                    "\n",
                    previousMessages.Select(
                        message =>
                            $"{GetRoleName(message.Role)}: {message.Content}"));

        return $$"""
        You are CareerAI, an AI career assistant.

        Your purpose is to help candidates with:
        - Career guidance
        - Job preparation
        - Resume improvement
        - Technical skills
        - Interview preparation
        - Learning roadmaps
        - Job search advice

        Rules:
        - Give practical and professional answers.
        - Keep answers clear and easy to understand.
        - Do not invent information about the candidate.
        - If you don't know something, say so.
        - Do not use unnecessary markdown.
        - Focus on the user's question.

        Previous Conversation:
        {{conversationHistory}}

        Current User Message:
        {{currentMessage}}

        Provide the best helpful response.
        """;
    }

    // ========================================
    // Convert Role To Text
    // ========================================

    private static string GetRoleName(
        AIMessageRole role)
    {
        return role switch
        {
            AIMessageRole.User =>
                "User",

            AIMessageRole.Assistant =>
                "Assistant",

            _ =>
                "Unknown"
        };
    }

    // ========================================
    // Map Message To DTO
    // ========================================

    private static AIMessageResponse
        MapMessageToResponse(
            AIMessage message)
    {
        return new AIMessageResponse
        {
            Id =
                message.Id,

            ConversationId =
                message.ConversationId,

            Role =
                GetRoleName(
                    message.Role),

            Content =
                message.Content,

            CreatedAt =
                message.CreatedAt
        };
    }
}