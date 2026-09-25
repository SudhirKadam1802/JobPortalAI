namespace CareerAI.Application.Features.AIInterview.DTOs;

public class StartInterviewRequest
{
    public string JobTitle { get; set; } = string.Empty;

    public int NumberOfQuestions { get; set; } = 5;
}