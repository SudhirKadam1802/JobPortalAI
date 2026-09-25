namespace CareerAI.Application.Features.JobRecommendations.DTOs;

public class JobRecommendationResponse
{
    public Guid Id { get; set; }

    public Guid JobId { get; set; }

    public string JobTitle { get; set; } = string.Empty;

    public string Location { get; set; } = string.Empty;

    public decimal MatchScore { get; set; }

    public List<string> MatchedSkills { get; set; } = new();

    public List<string> MissingSkills { get; set; } = new();

    public string Explanation { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}