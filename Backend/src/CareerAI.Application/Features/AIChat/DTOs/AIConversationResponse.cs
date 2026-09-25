namespace CareerAI.Application.Features.AIChat.DTOs;

public class AIConversationResponse
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public List<AIMessageResponse> Messages { get; set; }
        = new();
}