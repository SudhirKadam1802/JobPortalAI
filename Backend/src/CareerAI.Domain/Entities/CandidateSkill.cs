namespace CareerAI.Domain.Entities;

public class CandidateSkill
{
    public Guid CandidateId { get; set; }

    public Guid SkillId { get; set; }

    public Candidate Candidate { get; set; } = null!;

    public Skill Skill { get; set; } = null!;
}