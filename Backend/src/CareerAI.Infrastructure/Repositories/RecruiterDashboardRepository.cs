using CareerAI.Application.Features.RecruiterDashboard.DTOs;
using CareerAI.Application.Interfaces;
using CareerAI.Domain.Enums;
using CareerAI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CareerAI.Infrastructure.Repositories;

public class RecruiterDashboardRepository
    : IRecruiterDashboardRepository
{
    private readonly CareerAIDbContext _context;

    public RecruiterDashboardRepository(
        CareerAIDbContext context)
    {
        _context = context;
    }

    public async Task<RecruiterDashboardResponse> GetDashboardAsync(
        Guid userId)
    {
        // Find the Recruiter record using the logged-in User ID.
        var recruiter = await _context.Recruiters
            .FirstOrDefaultAsync(
                recruiter => recruiter.UserId == userId);

        if (recruiter is null)
        {
            return new RecruiterDashboardResponse();
        }

        // Get jobs that belong to this Recruiter.
        var jobs = _context.Jobs
            .Where(job => job.RecruiterId == recruiter.Id);

        // Get the IDs of this recruiter's jobs.
        var jobIds = await jobs
            .Select(job => job.Id)
            .ToListAsync();

        // Get applications submitted for this recruiter's jobs.
        var applications = _context.Applications
            .Where(application =>
                jobIds.Contains(application.JobId));

        var totalJobs =
            await jobs.CountAsync();

        var activeJobs =
            await jobs.CountAsync(
                job => job.ApplicationDeadline >= DateTime.UtcNow);

        var totalApplications =
            await applications.CountAsync();

        var appliedApplications =
            await applications.CountAsync(
                application =>
                    application.Status ==
                    ApplicationStatus.Applied);

        var shortlistedApplications =
            await applications.CountAsync(
                application =>
                    application.Status ==
                    ApplicationStatus.Shortlisted);

        var interviewApplications =
            await applications.CountAsync(
                application =>
                    application.Status ==
                    ApplicationStatus.Interview);

        var rejectedApplications =
            await applications.CountAsync(
                application =>
                    application.Status ==
                    ApplicationStatus.Rejected);

        var hiredApplications =
            await applications.CountAsync(
                application =>
                    application.Status ==
                    ApplicationStatus.Hired);

        return new RecruiterDashboardResponse
        {
            TotalJobs = totalJobs,
            ActiveJobs = activeJobs,
            TotalApplications = totalApplications,
            AppliedApplications = appliedApplications,
            ShortlistedApplications = shortlistedApplications,
            InterviewApplications = interviewApplications,
            RejectedApplications = rejectedApplications,
            HiredApplications = hiredApplications
        };
    }
}