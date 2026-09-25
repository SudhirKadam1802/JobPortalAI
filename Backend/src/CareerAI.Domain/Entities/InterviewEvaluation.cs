namespace CareerAI.Domain.Entities;

public class InterviewEvaluation
{
    public Guid Id { get; set; }

    public Guid InterviewQuestionId { get; set; }

    public int Score { get; set; }

    public string? Feedback { get; set; }

    public InterviewQuestion InterviewQuestion { get; set; } = null!;
}