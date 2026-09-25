namespace CareerAI.Domain.Entities;

public class Skill
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public ICollection<JobSkill> JobSkills { get; set; } = new List<JobSkill>();
}