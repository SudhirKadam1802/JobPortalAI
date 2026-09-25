namespace CareerAI.Domain.Entities;

public class Interview
{
    public Guid Id { get; set; }

    public Guid CandidateId { get; set; }

    public string InterviewType { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public DateTime? ScheduledAt { get; set; }

    public DateTime? StartedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public Candidate Candidate { get; set; } = null!;

    public ICollection<InterviewQuestion> Questions { get; set; }
        = new List<InterviewQuestion>();
}