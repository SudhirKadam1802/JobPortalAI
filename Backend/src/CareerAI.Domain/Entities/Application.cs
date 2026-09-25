using CareerAI.Domain.Enums;

namespace CareerAI.Domain.Entities;

public class Application
{
    public Guid Id { get; set; }

    public Guid CandidateId { get; set; }

    public Guid JobId { get; set; }

    public ApplicationStatus Status { get; set; }

    public string? CoverLetter { get; set; }

    public DateTime AppliedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Candidate Candidate { get; set; } = null!;

    public Job Job { get; set; } = null!;
}