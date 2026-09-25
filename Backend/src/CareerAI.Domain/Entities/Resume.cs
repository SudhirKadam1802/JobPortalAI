namespace CareerAI.Domain.Entities;

public class Resume
{
    public Guid Id { get; set; }

    public Guid CandidateId { get; set; }

    public string FileName { get; set; } = string.Empty;

    public string FilePath { get; set; } = string.Empty;

    public string FileType { get; set; } = string.Empty;

    public DateTime UploadedAt { get; set; }

    public Candidate Candidate { get; set; } = null!;
}