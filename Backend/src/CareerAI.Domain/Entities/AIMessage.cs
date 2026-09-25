using CareerAI.Domain.Enums;

namespace CareerAI.Domain.Entities;

public class AIMessage
{
    public Guid Id { get; set; }

    public Guid ConversationId { get; set; }

    public AIMessageRole Role { get; set; }

    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public AIConversation Conversation { get; set; } = null!;
}