namespace CareerAI.Domain.Entities;

public class JobApplicationNote
{
    public Guid Id { get; set; }

    public Guid ApplicationId { get; set; }

    public Guid RecruiterId { get; set; }

    public string Note { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public Application Application { get; set; } = null!;

    public Recruiter Recruiter { get; set; } = null!;
}