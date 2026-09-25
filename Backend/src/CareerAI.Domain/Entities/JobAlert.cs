namespace CareerAI.Domain.Entities;

public class JobAlert
{
    public Guid Id { get; set; }

    public Guid CandidateId { get; set; }

    public string Keyword { get; set; } = string.Empty;

    public string? Location { get; set; }

    public decimal? MinimumSalary { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Candidate Candidate { get; set; } = null!;
}