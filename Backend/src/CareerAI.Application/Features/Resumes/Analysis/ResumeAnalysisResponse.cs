namespace CareerAI.Application.Features.Resumes.Analysis;

public class ResumeAnalysisResponse
{
    public Guid Id { get; set; }

    public Guid ResumeId { get; set; }

    public int Score { get; set; }

    public string Summary { get; set; } = string.Empty;

    public List<string> Skills { get; set; } = new();

    public string Education { get; set; } = string.Empty;

    public string Experience { get; set; } = string.Empty;

    public List<string> Suggestions { get; set; } = new();

    public DateTime AnalyzedAt { get; set; }
}