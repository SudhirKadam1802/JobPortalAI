namespace CareerAI.Domain.Entities;

public class Experience
{
    public Guid Id { get; set; }

    public Guid CandidateId { get; set; }

    public string JobTitle { get; set; } = string.Empty;

    public string CompanyName { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public bool IsCurrent { get; set; }

    public Candidate Candidate { get; set; } = null!;
}