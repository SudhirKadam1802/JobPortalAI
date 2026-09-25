namespace CareerAI.Application.Features.Jobs.DTOs;

public class CreateJobRequest
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Location { get; set; } = string.Empty;

    public string EmploymentType { get; set; } = string.Empty;

    public int MinimumExperience { get; set; }

    public int MaximumExperience { get; set; }

    public decimal? MinimumSalary { get; set; }

    public decimal? MaximumSalary { get; set; }

    public DateTime ApplicationDeadline { get; set; }
}