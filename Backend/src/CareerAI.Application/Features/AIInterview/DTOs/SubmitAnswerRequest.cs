namespace CareerAI.Application.Features.AIInterview.DTOs;

public class SubmitAnswerRequest
{
    public Guid QuestionId { get; set; }

    public string Answer { get; set; } = string.Empty;
}