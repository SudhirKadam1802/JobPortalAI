using CareerAI.Application.Features.JobAlerts.DTOs;
using CareerAI.Application.Interfaces;
using CareerAI.Domain.Entities;

namespace CareerAI.Application.Features.JobAlerts.Services;

public class JobAlertService : IJobAlertService
{
    private readonly IJobAlertRepository _jobAlertRepository;
    private readonly ICandidateRepository _candidateRepository;

    public JobAlertService(
        IJobAlertRepository jobAlertRepository,
        ICandidateRepository candidateRepository)
    {
        _jobAlertRepository = jobAlertRepository;
        _candidateRepository = candidateRepository;
    }

    // =========================================================
    // CREATE
    // =========================================================

    public async Task<JobAlertResponse> CreateAsync(
        Guid userId,
        CreateJobAlertRequest request)
    {
        var candidate =
            await _candidateRepository.GetByUserIdAsync(userId);

        if (candidate is null)
        {
            throw new KeyNotFoundException(
                "Candidate profile not found.");
        }

        if (string.IsNullOrWhiteSpace(request.Keyword) &&
            string.IsNullOrWhiteSpace(request.Location) &&
            request.MinimumSalary is null)
        {
            throw new ArgumentException(
                "At least one job alert filter is required.");
        }

        if (request.MinimumSalary < 0)
        {
            throw new ArgumentException(
                "Minimum salary cannot be negative.");
        }

        var jobAlert = new JobAlert
        {
            Id = Guid.NewGuid(),

            CandidateId = candidate.Id,

            Keyword =
                request.Keyword?.Trim() ?? string.Empty,

            Location =
                request.Location?.Trim(),

            MinimumSalary =
                request.MinimumSalary,

            IsActive =
                request.IsActive,

            CreatedAt =
                DateTime.UtcNow
        };

        await _jobAlertRepository.AddAsync(jobAlert);

        return MapToResponse(jobAlert);
    }


    // =========================================================
    // GET MY ALERTS
    // =========================================================

    public async Task<List<JobAlertResponse>> GetMyAlertsAsync(
        Guid userId)
    {
        var candidate =
            await _candidateRepository.GetByUserIdAsync(userId);

        if (candidate is null)
        {
            throw new KeyNotFoundException(
                "Candidate profile not found.");
        }

        var jobAlerts =
            await _jobAlertRepository
                .GetByCandidateIdAsync(candidate.Id);

        return jobAlerts
            .Select(MapToResponse)
            .ToList();
    }


    // =========================================================
    // GET BY ID
    // =========================================================

    public async Task<JobAlertResponse?> GetByIdAsync(
        Guid userId,
        Guid id)
    {
        var candidate =
            await _candidateRepository.GetByUserIdAsync(userId);

        if (candidate is null)
        {
            throw new KeyNotFoundException(
                "Candidate profile not found.");
        }

        var jobAlert =
            await _jobAlertRepository.GetByIdAsync(id);

        if (jobAlert is null)
        {
            return null;
        }

        if (jobAlert.CandidateId != candidate.Id)
        {
            throw new UnauthorizedAccessException(
                "You are not authorized to access this job alert.");
        }

        return MapToResponse(jobAlert);
    }


    // =========================================================
    // UPDATE
    // =========================================================

    public async Task<JobAlertResponse?> UpdateAsync(
        Guid userId,
        Guid id,
        UpdateJobAlertRequest request)
    {
        var candidate =
            await _candidateRepository.GetByUserIdAsync(userId);

        if (candidate is null)
        {
            throw new KeyNotFoundException(
                "Candidate profile not found.");
        }

        var jobAlert =
            await _jobAlertRepository.GetByIdAsync(id);

        if (jobAlert is null)
        {
            return null;
        }

        if (jobAlert.CandidateId != candidate.Id)
        {
            throw new UnauthorizedAccessException(
                "You are not authorized to update this job alert.");
        }

        if (string.IsNullOrWhiteSpace(request.Keyword) &&
            string.IsNullOrWhiteSpace(request.Location) &&
            request.MinimumSalary is null)
        {
            throw new ArgumentException(
                "At least one job alert filter is required.");
        }

        if (request.MinimumSalary < 0)
        {
            throw new ArgumentException(
                "Minimum salary cannot be negative.");
        }

        jobAlert.Keyword =
            request.Keyword?.Trim() ?? string.Empty;

        jobAlert.Location =
            request.Location?.Trim();

        jobAlert.MinimumSalary =
            request.MinimumSalary;

        jobAlert.IsActive =
            request.IsActive;

        jobAlert.UpdatedAt =
            DateTime.UtcNow;

        await _jobAlertRepository.UpdateAsync(jobAlert);

        return MapToResponse(jobAlert);
    }


    // =========================================================
    // DELETE
    // =========================================================

    public async Task<bool> DeleteAsync(
        Guid userId,
        Guid id)
    {
        var candidate =
            await _candidateRepository.GetByUserIdAsync(userId);

        if (candidate is null)
        {
            throw new KeyNotFoundException(
                "Candidate profile not found.");
        }

        var jobAlert =
            await _jobAlertRepository.GetByIdAsync(id);

        if (jobAlert is null)
        {
            return false;
        }

        if (jobAlert.CandidateId != candidate.Id)
        {
            throw new UnauthorizedAccessException(
                "You are not authorized to delete this job alert.");
        }

        await _jobAlertRepository.DeleteAsync(jobAlert);

        return true;
    }


    // =========================================================
    // MAPPING
    // =========================================================

    private static JobAlertResponse MapToResponse(
        JobAlert jobAlert)
    {
        return new JobAlertResponse
        {
            Id =
                jobAlert.Id,

            Keyword =
                jobAlert.Keyword,

            Location =
                jobAlert.Location,

            MinimumSalary =
                jobAlert.MinimumSalary,

            IsActive =
                jobAlert.IsActive,

            CreatedAt =
                jobAlert.CreatedAt,

            UpdatedAt =
                jobAlert.UpdatedAt
        };
    }
}