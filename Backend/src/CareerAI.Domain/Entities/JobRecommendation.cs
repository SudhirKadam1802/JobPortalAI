namespace CareerAI.Domain.Entities;

public class JobRecommendation
{
    public Guid Id { get; set; }

    public Guid CandidateId { get; set; }

    public Guid JobId { get; set; }

    public decimal MatchScore { get; set; }

    public string? MatchedSkills { get; set; }

    public string? MissingSkills { get; set; }

    public string? Explanation { get; set; }

    public DateTime CreatedAt { get; set; }

    public Candidate Candidate { get; set; } = null!;

    public Job Job { get; set; } = null!;
}