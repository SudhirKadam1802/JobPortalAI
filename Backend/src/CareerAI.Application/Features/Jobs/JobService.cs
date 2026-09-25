
using CareerAI.Application.Features.Jobs.DTOs;
using CareerAI.Application.Interfaces;
using CareerAI.Domain.Entities;

namespace CareerAI.Application.Features.Jobs;

public class JobService : IJobService
{
    private readonly IJobRepository _jobRepository;
    private readonly IRecruiterRepository _recruiterRepository;

    public JobService(
        IJobRepository jobRepository,
        IRecruiterRepository recruiterRepository)
    {
        _jobRepository = jobRepository;
        _recruiterRepository = recruiterRepository;
    }

    // ========================================
    // Create Job
    // ========================================

    public async Task<JobResponse> CreateAsync(
        Guid userId,
        CreateJobRequest request)
    {
        var recruiter = await _recruiterRepository
            .GetByUserIdAsync(userId);

        if (recruiter is null)
        {
            throw new UnauthorizedAccessException(
                "Recruiter profile was not found.");
        }

        // Validate company name entered for this job
        if (string.IsNullOrWhiteSpace(request.CompanyName))
        {
            throw new Exception(
                "Company name is required.");
        }

        if (request.CompanyName.Trim().Length > 150)
        {
            throw new Exception(
                "Company name cannot exceed 150 characters.");
        }

        if (string.IsNullOrWhiteSpace(request.Title))
        {
            throw new Exception("Job title is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Description))
        {
            throw new Exception("Job description is required.");
        }

        if (request.MinimumExperience < 0)
        {
            throw new Exception(
                "Minimum experience cannot be negative.");
        }

        if (request.MaximumExperience < request.MinimumExperience)
        {
            throw new Exception(
                "Maximum experience cannot be less than minimum experience.");
        }

        if (request.MinimumSalary.HasValue &&
            request.MaximumSalary.HasValue &&
            request.MaximumSalary < request.MinimumSalary)
        {
            throw new Exception(
                "Maximum salary cannot be less than minimum salary.");
        }

        var job = new Job
        {
            Id = Guid.NewGuid(),

            RecruiterId = recruiter.Id,

            // Company name belongs to this individual job
            CompanyName = request.CompanyName.Trim(),

            // Preserve existing recruiter relationship
            Recruiter = recruiter,

            Title = request.Title.Trim(),

            Description = request.Description.Trim(),

            Location = request.Location.Trim(),

            EmploymentType = request.EmploymentType.Trim(),

            MinimumExperience = request.MinimumExperience,

            MaximumExperience = request.MaximumExperience,

            MinimumSalary = request.MinimumSalary,

            MaximumSalary = request.MaximumSalary,

            RequiredEducation = request.RequiredEducation,

            ApplicationDeadline = request.ApplicationDeadline,

            CreatedAt = DateTime.UtcNow
        };

        await _jobRepository.AddAsync(job);

        await _jobRepository.SaveChangesAsync();

        return MapToResponse(job);
    }

    // ========================================
    // Get All Jobs
    // ========================================

    public async Task<List<JobResponse>> GetAllAsync()
    {
        var jobs = await _jobRepository.GetAllAsync();

        return jobs
            .Select(MapToResponse)
            .ToList();
    }

    // ========================================
    // Get Job By Id
    // ========================================

    public async Task<JobResponse?> GetByIdAsync(Guid id)
    {
        var job = await _jobRepository.GetByIdAsync(id);

        if (job is null)
        {
            return null;
        }

        return MapToResponse(job);
    }

    // ========================================
    // Update Job
    // ========================================

    public async Task<JobResponse?> UpdateAsync(
        Guid userId,
        Guid id,
        UpdateJobRequest request)
    {
        var job = await _jobRepository.GetByIdAsync(id);

        if (job is null)
        {
            return null;
        }

        var recruiter = await _recruiterRepository
            .GetByUserIdAsync(userId);

        if (recruiter is null)
        {
            throw new UnauthorizedAccessException(
                "Recruiter profile was not found.");
        }

        // Preserve existing ownership authorization
        if (job.RecruiterId != recruiter.Id)
        {
            throw new UnauthorizedAccessException(
                "You are not authorized to update this job.");
        }

        // Validate company name during update
        if (string.IsNullOrWhiteSpace(request.CompanyName))
        {
            throw new Exception(
                "Company name is required.");
        }

        if (request.CompanyName.Trim().Length > 150)
        {
            throw new Exception(
                "Company name cannot exceed 150 characters.");
        }

        if (string.IsNullOrWhiteSpace(request.Title))
        {
            throw new Exception("Job title is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Description))
        {
            throw new Exception("Job description is required.");
        }

        if (request.MinimumExperience < 0)
        {
            throw new Exception(
                "Minimum experience cannot be negative.");
        }

        if (request.MaximumExperience < request.MinimumExperience)
        {
            throw new Exception(
                "Maximum experience cannot be less than minimum experience.");
        }

        if (request.MinimumSalary.HasValue &&
            request.MaximumSalary.HasValue &&
            request.MaximumSalary < request.MinimumSalary)
        {
            throw new Exception(
                "Maximum salary cannot be less than minimum salary.");
        }

        // Update company name stored on this job
        job.CompanyName = request.CompanyName.Trim();

        job.Title = request.Title.Trim();

        job.Description = request.Description.Trim();

        job.Location = request.Location.Trim();

        job.EmploymentType = request.EmploymentType.Trim();

        job.MinimumExperience = request.MinimumExperience;

        job.MaximumExperience = request.MaximumExperience;

        job.MinimumSalary = request.MinimumSalary;

        job.MaximumSalary = request.MaximumSalary;

        job.RequiredEducation = request.RequiredEducation;

        job.ApplicationDeadline = request.ApplicationDeadline;

        job.UpdatedAt = DateTime.UtcNow;

        _jobRepository.Update(job);

        await _jobRepository.SaveChangesAsync();

        // Reload job with related data
        var updatedJob = await _jobRepository.GetByIdAsync(id);

        return updatedJob is null
            ? null
            : MapToResponse(updatedJob);
    }

    // ========================================
    // Delete Job
    // ========================================

    public async Task<bool> DeleteAsync(
        Guid userId,
        Guid id)
    {
        var job = await _jobRepository.GetByIdAsync(id);

        if (job is null)
        {
            return false;
        }

        var recruiter = await _recruiterRepository
            .GetByUserIdAsync(userId);

        if (recruiter is null)
        {
            throw new UnauthorizedAccessException(
                "Recruiter profile was not found.");
        }

        // Preserve existing ownership authorization
        if (job.RecruiterId != recruiter.Id)
        {
            throw new UnauthorizedAccessException(
                "You are not authorized to delete this job.");
        }

        _jobRepository.Delete(job);

        await _jobRepository.SaveChangesAsync();

        return true;
    }

    // ========================================
    // Entity -> Response DTO
    // ========================================

    private static JobResponse MapToResponse(Job job)
    {
        return new JobResponse
        {
            Id = job.Id,

            RecruiterId = job.RecruiterId,

            // Company name stored on the Job entity
            CompanyName = job.CompanyName,

            Title = job.Title,

            Description = job.Description,

            Location = job.Location,

            EmploymentType = job.EmploymentType,

            MinimumExperience = job.MinimumExperience,

            MaximumExperience = job.MaximumExperience,

            MinimumSalary = job.MinimumSalary,

            MaximumSalary = job.MaximumSalary,

            RequiredEducation = job.RequiredEducation,

            ApplicationDeadline = job.ApplicationDeadline,

            CreatedAt = job.CreatedAt,

            UpdatedAt = job.UpdatedAt,

            // Map existing job skills into response DTOs
            JobSkills = job.JobSkills
                .Where(js => js.Skill != null)
                .Select(js => new JobSkillResponse
                {
                    JobId = js.JobId,

                    SkillId = js.SkillId,

                    SkillName = js.Skill.Name
                })
                .ToList()
        };
    }
}