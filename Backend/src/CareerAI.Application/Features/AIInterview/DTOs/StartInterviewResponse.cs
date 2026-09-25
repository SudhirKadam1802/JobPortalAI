namespace CareerAI.Application.Features.AIInterview.DTOs;

public class StartInterviewResponse
{
    public Guid InterviewId { get; set; }

    public Guid QuestionId { get; set; }

    public string Question { get; set; } = string.Empty;

    public int QuestionNumber { get; set; }

    public int TotalQuestions { get; set; }
}