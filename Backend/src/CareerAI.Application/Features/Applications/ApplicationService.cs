using CareerAI.Application.Features.Applications.DTOs;
using CareerAI.Application.Features.Notifications.DTOs;
using CareerAI.Application.Interfaces;
using CareerAI.Domain.Enums;

using ApplicationEntity = CareerAI.Domain.Entities.Application;

namespace CareerAI.Application.Features.Applications;

public class ApplicationService : IApplicationService
{
    private readonly IApplicationRepository _applicationRepository;
    private readonly ICandidateRepository _candidateRepository;
    private readonly IRecruiterRepository _recruiterRepository;
    private readonly IJobRepository _jobRepository;
    private readonly INotificationService _notificationService;

    public ApplicationService(
        IApplicationRepository applicationRepository,
        ICandidateRepository candidateRepository,
        IRecruiterRepository recruiterRepository,
        IJobRepository jobRepository,
        INotificationService notificationService)
    {
        _applicationRepository = applicationRepository;
        _candidateRepository = candidateRepository;
        _recruiterRepository = recruiterRepository;
        _jobRepository = jobRepository;
        _notificationService = notificationService;
    }

    public async Task<ApplicationResponse> ApplyAsync(
        Guid userId,
        CreateApplicationRequest request)
    {
        // 1. Find candidate profile
        var candidate = await _candidateRepository
            .GetByUserIdAsync(userId);

        if (candidate is null)
        {
            throw new UnauthorizedAccessException(
                "Candidate profile was not found.");
        }

        // 2. Find job
        var job = await _jobRepository
            .GetByIdAsync(request.JobId);

        if (job is null)
        {
            throw new Exception("Job not found.");
        }

        // 3. Check application deadline
        if (job.ApplicationDeadline < DateTime.UtcNow)
        {
            throw new Exception(
                "Application deadline has passed.");
        }

        // 4. Check duplicate application
        var alreadyApplied = await _applicationRepository
            .ExistsAsync(candidate.Id, request.JobId);

        if (alreadyApplied)
        {
            throw new Exception(
                "You have already applied for this job.");
        }

        // 5. Create application
        var application = new ApplicationEntity
        {
            Id = Guid.NewGuid(),
            CandidateId = candidate.Id,
            JobId = job.Id,
            Status = ApplicationStatus.Applied,
            CoverLetter = request.CoverLetter?.Trim(),
            AppliedAt = DateTime.UtcNow
        };

        await _applicationRepository.AddAsync(application);

        await _applicationRepository.SaveChangesAsync();

        // 6. Find recruiter who owns the job
        var recruiter = await _recruiterRepository
            .GetByIdAsync(job.RecruiterId);

        // 7. Create notification for recruiter
        if (recruiter is not null)
        {
            var notificationRequest =
                new CreateNotificationRequest
                {
                    UserId = recruiter.UserId,

                    Title = "New Job Application",

                    Message =
                        $"{candidate.User.FirstName} " +
                        $"{candidate.User.LastName} " +
                        $"applied for {job.Title}."
                };

            await _notificationService
                .CreateAsync(notificationRequest);
        }

        // 8. Return response
        return new ApplicationResponse
        {
            Id = application.Id,
            CandidateId = application.CandidateId,
            JobId = application.JobId,
            JobTitle = job.Title,
            Status = (int)application.Status,
            StatusName = application.Status.ToString(),
            CoverLetter = application.CoverLetter,
            AppliedAt = application.AppliedAt,
            UpdatedAt = application.UpdatedAt
        };
    }

    public async Task<List<ApplicationResponse>> GetMyApplicationsAsync(
        Guid userId)
    {
        // Find candidate
        var candidate = await _candidateRepository
            .GetByUserIdAsync(userId);

        if (candidate is null)
        {
            throw new UnauthorizedAccessException(
                "Candidate profile was not found.");
        }

        var applications = await _applicationRepository
            .GetByCandidateIdAsync(candidate.Id);

        return applications
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<List<ApplicationResponse>> GetJobApplicationsAsync(
        Guid userId,
        Guid jobId)
    {
        // Find recruiter
        var recruiter = await _recruiterRepository
            .GetByUserIdAsync(userId);

        if (recruiter is null)
        {
            throw new UnauthorizedAccessException(
                "Recruiter profile was not found.");
        }

        // Find job
        var job = await _jobRepository
            .GetByIdAsync(jobId);

        if (job is null)
        {
            throw new Exception("Job not found.");
        }

        // Make sure recruiter owns the job
        if (job.RecruiterId != recruiter.Id)
        {
            throw new UnauthorizedAccessException(
                "You are not authorized to view these applications.");
        }

        var applications = await _applicationRepository
            .GetByJobIdAsync(jobId);

        return applications
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<ApplicationResponse?> GetByIdAsync(
        Guid userId,
        Guid applicationId)
    {
        var application = await _applicationRepository
            .GetByIdAsync(applicationId);

        if (application is null)
        {
            return null;
        }

        // Candidate can view their own application
        var candidate = await _candidateRepository
            .GetByUserIdAsync(userId);

        if (candidate is not null)
        {
            if (application.CandidateId != candidate.Id)
            {
                throw new UnauthorizedAccessException(
                    "You are not authorized to view this application.");
            }

            return MapToResponse(application);
        }

        // Recruiter can view applications for their job
        var recruiter = await _recruiterRepository
            .GetByUserIdAsync(userId);

        if (recruiter is not null)
        {
            if (application.Job.RecruiterId != recruiter.Id)
            {
                throw new UnauthorizedAccessException(
                    "You are not authorized to view this application.");
            }

            return MapToResponse(application);
        }

        throw new UnauthorizedAccessException(
            "User profile was not found.");
    }

    public async Task<ApplicationResponse?> UpdateStatusAsync(
        Guid userId,
        Guid applicationId,
        UpdateApplicationStatusRequest request)
    {
        // 1. Find recruiter
        var recruiter = await _recruiterRepository
            .GetByUserIdAsync(userId);

        if (recruiter is null)
        {
            throw new UnauthorizedAccessException(
                "Recruiter profile was not found.");
        }

        // 2. Validate status
        if (!Enum.IsDefined(
                typeof(ApplicationStatus),
                request.Status))
        {
            throw new Exception(
                "Invalid application status.");
        }

        // 3. Find application
        var application = await _applicationRepository
            .GetByIdAsync(applicationId);

        if (application is null)
        {
            return null;
        }

        // 4. Make sure recruiter owns the job
        if (application.Job.RecruiterId != recruiter.Id)
        {
            throw new UnauthorizedAccessException(
                "You are not authorized to update this application.");
        }

        // 5. Update status
        application.Status =
            (ApplicationStatus)request.Status;

        application.UpdatedAt = DateTime.UtcNow;

        _applicationRepository.Update(application);

        await _applicationRepository.SaveChangesAsync();

        return MapToResponse(application);
    }

    private static ApplicationResponse MapToResponse(
        ApplicationEntity application)
    {
        return new ApplicationResponse
        {
            Id = application.Id,
            CandidateId = application.CandidateId,
            JobId = application.JobId,

            JobTitle = application.Job?.Title ?? string.Empty,

            CandidateName =
                application.Candidate?.User is not null
                    ? $"{application.Candidate.User.FirstName} {application.Candidate.User.LastName}"
                    : null,

            CandidateEmail =
                application.Candidate?.User?.Email,

            Status = (int)application.Status,

            StatusName =
                application.Status.ToString(),

            CoverLetter =
                application.CoverLetter,

            AppliedAt =
                application.AppliedAt,

            UpdatedAt =
                application.UpdatedAt
        };
    }
}