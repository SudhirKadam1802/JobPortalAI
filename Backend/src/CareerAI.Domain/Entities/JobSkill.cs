namespace CareerAI.Domain.Entities;

public class JobSkill
{
    public Guid JobId { get; set; }

    public Guid SkillId { get; set; }

    public Job Job { get; set; } = null!;

    public Skill Skill { get; set; } = null!;
}