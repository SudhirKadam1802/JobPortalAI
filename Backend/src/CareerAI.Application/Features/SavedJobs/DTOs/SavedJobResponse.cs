namespace CareerAI.Application.Features.SavedJobs.DTOs;

public class SavedJobResponse
{
    public Guid Id { get; set; }

    public Guid JobId { get; set; }

    public string JobTitle { get; set; } = string.Empty;

    public string Location { get; set; } = string.Empty;

    public string EmploymentType { get; set; } = string.Empty;

    public int MinimumExperience { get; set; }

    public int MaximumExperience { get; set; }

    public decimal? MinimumSalary { get; set; }

    public decimal? MaximumSalary { get; set; }

    public DateTime ApplicationDeadline { get; set; }

    public DateTime SavedAt { get; set; }
}