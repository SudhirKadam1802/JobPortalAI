namespace CareerAI.Application.Features.AIInterview.DTOs;

public class SubmitAnswerResponse
{
    public Guid QuestionId { get; set; }

    public int Score { get; set; }

    public string Feedback { get; set; } = string.Empty;

    public bool IsInterviewCompleted { get; set; }

    public Guid? NextQuestionId { get; set; }

    public string? NextQuestion { get; set; }

    public int? NextQuestionNumber { get; set; }

    public int? TotalQuestions { get; set; }
}