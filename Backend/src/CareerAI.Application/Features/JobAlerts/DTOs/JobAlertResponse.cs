namespace CareerAI.Application.Features.JobAlerts.DTOs;

public class JobAlertResponse
{
    public Guid Id { get; set; }

    public string Keyword { get; set; } = string.Empty;

    public string? Location { get; set; }

    public decimal? MinimumSalary { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}