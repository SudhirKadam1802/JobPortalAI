using CareerAI.Application.Features.Jobs.DTOs;

namespace CareerAI.Application.Features.Jobs.DTOs;

public class JobResponse
{
    public Guid Id { get; set; }

    public Guid RecruiterId { get; set; }

    public string CompanyName { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Location { get; set; } = string.Empty;

    public string EmploymentType { get; set; } = string.Empty;

    public int MinimumExperience { get; set; }

    public int MaximumExperience { get; set; }

    public decimal? MinimumSalary { get; set; }

    public decimal? MaximumSalary { get; set; }

    public string? RequiredEducation { get; set; }

    public DateTime ApplicationDeadline { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public List<JobSkillResponse> JobSkills { get; set; } = new();
}