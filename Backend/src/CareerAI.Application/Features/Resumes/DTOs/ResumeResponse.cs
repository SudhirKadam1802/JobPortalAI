namespace CareerAI.Application.Features.Resumes.DTOs;

public class ResumeResponse
{
    public Guid Id { get; set; }

    public Guid CandidateId { get; set; }

    public string FileName { get; set; } = string.Empty;

    public string FileType { get; set; } = string.Empty;

    public DateTime UploadedAt { get; set; }
}