namespace CareerAI.Domain.Entities;

public class InterviewQuestion
{
    public Guid Id { get; set; }

    public Guid InterviewId { get; set; }

    public string Question { get; set; } = string.Empty;

    public string? ExpectedAnswer { get; set; }

    public int Order { get; set; }

    public Interview Interview { get; set; } = null!;

    public InterviewEvaluation? Evaluation { get; set; }
}