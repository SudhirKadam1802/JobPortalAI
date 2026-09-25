namespace CareerAI.Domain.Entities;

public class Project
{
    public Guid Id { get; set; }

    public Guid CandidateId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? Technologies { get; set; }

    public string? ProjectUrl { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public Candidate Candidate { get; set; } = null!;
}