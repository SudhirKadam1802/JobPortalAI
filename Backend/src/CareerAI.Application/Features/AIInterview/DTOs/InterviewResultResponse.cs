namespace CareerAI.Application.Features.AIInterview.DTOs;

public class InterviewResultResponse
{
    public Guid InterviewId { get; set; }

    public int TotalScore { get; set; }

    public int MaximumScore { get; set; }

    public decimal Percentage { get; set; }

    public string Strengths { get; set; } = string.Empty;

    public string Improvements { get; set; } = string.Empty;
}