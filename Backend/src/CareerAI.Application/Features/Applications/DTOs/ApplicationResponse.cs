namespace CareerAI.Application.Features.Applications.DTOs;

public class ApplicationResponse
{
    public Guid Id { get; set; }

    public Guid CandidateId { get; set; }

    public Guid JobId { get; set; }

    public string JobTitle { get; set; } = string.Empty;

    public string? CandidateName { get; set; }

    public string? CandidateEmail { get; set; }

    public int Status { get; set; }

    public string StatusName { get; set; } = string.Empty;

    public string? CoverLetter { get; set; }

    public DateTime AppliedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}