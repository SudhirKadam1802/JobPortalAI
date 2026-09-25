namespace CareerAI.Application.Features.Jobs.DTOs;

public class JobSkillResponse
{
    public Guid JobId { get; set; }

    public Guid SkillId { get; set; }

    public string SkillName { get; set; } = string.Empty;
}