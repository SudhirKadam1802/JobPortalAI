namespace CareerAI.Domain.Entities;

public class Candidate
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Location { get; set; }

    public string? Bio { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public User User { get; set; } = null!;

    public ICollection<CandidateSkill> CandidateSkills { get; set; }
        = new List<CandidateSkill>();

    public ICollection<Education> Educations { get; set; }
    = new List<Education>();
    public ICollection<Experience> Experiences { get; set; }
        = new List<Experience>();
}