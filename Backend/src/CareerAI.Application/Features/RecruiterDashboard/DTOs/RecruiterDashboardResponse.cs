namespace CareerAI.Application.Features.RecruiterDashboard.DTOs;

public class RecruiterDashboardResponse
{
    public int TotalJobs { get; set; }

    public int ActiveJobs { get; set; }

    public int TotalApplications { get; set; }

    public int AppliedApplications { get; set; }

    public int ShortlistedApplications { get; set; }

    public int InterviewApplications { get; set; }

    public int RejectedApplications { get; set; }

    public int HiredApplications { get; set; }
}