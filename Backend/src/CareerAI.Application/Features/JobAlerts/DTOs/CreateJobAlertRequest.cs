namespace CareerAI.Application.Features.JobAlerts.DTOs;

public class CreateJobAlertRequest
{
    public string? Keyword { get; set; }

    public string? Location { get; set; }

    public decimal? MinimumSalary { get; set; }

    public bool IsActive { get; set; } = true;
}