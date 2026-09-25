namespace CareerAI.Domain.Entities;

public class Education
{
    public Guid Id { get; set; }

    public Guid CandidateId { get; set; }

    public string Degree { get; set; } = string.Empty;

    public string Institution { get; set; } = string.Empty;

    public string? FieldOfStudy { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public decimal? Grade { get; set; }

    public Candidate Candidate { get; set; } = null!;
}