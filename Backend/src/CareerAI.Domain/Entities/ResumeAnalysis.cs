namespace CareerAI.Domain.Entities;

public class ResumeAnalysis
{
    public Guid Id { get; set; }

    public Guid ResumeId { get; set; }

    public int Score { get; set; }

    public string? Summary { get; set; }

    public string? ExtractedSkills { get; set; }

    public string? ExtractedEducation { get; set; }

    public string? ExtractedExperience { get; set; }

    public string? Suggestions { get; set; }

    public DateTime AnalyzedAt { get; set; }

    public Resume Resume { get; set; } = null!;
}