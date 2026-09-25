using CareerAI.Application.Features.SavedJobs.DTOs;
using CareerAI.Application.Interfaces;
using CareerAI.Domain.Entities;

namespace CareerAI.Application.Features.SavedJobs.Services;

public class SavedJobService : ISavedJobService
{
    private readonly ISavedJobRepository _savedJobRepository;
    private readonly ICandidateRepository _candidateRepository;
    private readonly IJobRepository _jobRepository;

    public SavedJobService(
        ISavedJobRepository savedJobRepository,
        ICandidateRepository candidateRepository,
        IJobRepository jobRepository)
    {
        _savedJobRepository = savedJobRepository;
        _candidateRepository = candidateRepository;
        _jobRepository = jobRepository;
    }


    // =========================================================
    // SAVE JOB
    // =========================================================

    public async Task<SavedJobResponse> SaveAsync(
        Guid userId,
        Guid jobId)
    {
        // -----------------------------------------------------
        // Get candidate
        // -----------------------------------------------------

        var candidate =
            await _candidateRepository.GetByUserIdAsync(userId);

        if (candidate is null)
        {
            throw new KeyNotFoundException(
                "Candidate profile not found.");
        }


        // -----------------------------------------------------
        // Check job
        // -----------------------------------------------------

        var job =
            await _jobRepository.GetByIdAsync(jobId);

        if (job is null)
        {
            throw new KeyNotFoundException(
                "Job not found.");
        }


        // -----------------------------------------------------
        // Check whether already saved
        // -----------------------------------------------------

        var existing =
            await _savedJobRepository.GetAsync(
                candidate.Id,
                jobId);

        if (existing is not null)
        {
            throw new ArgumentException(
                "You have already saved this job.");
        }


        // -----------------------------------------------------
        // Create saved job
        // -----------------------------------------------------

        var savedJob = new SavedJob
        {
            Id = Guid.NewGuid(),

            CandidateId = candidate.Id,

            JobId = jobId,

            SavedAt = DateTime.UtcNow
        };


        // -----------------------------------------------------
        // Save to database
        // -----------------------------------------------------

        await _savedJobRepository.AddAsync(savedJob);


        // -----------------------------------------------------
        // Return response
        // -----------------------------------------------------

        savedJob.Job = job;

        return MapToResponse(savedJob);
    }


    // =========================================================
    // GET MY SAVED JOBS
    // =========================================================

    public async Task<List<SavedJobResponse>>
        GetMySavedJobsAsync(Guid userId)
    {
        // -----------------------------------------------------
        // Get candidate
        // -----------------------------------------------------

        var candidate =
            await _candidateRepository.GetByUserIdAsync(userId);

        if (candidate is null)
        {
            throw new KeyNotFoundException(
                "Candidate profile not found.");
        }


        // -----------------------------------------------------
        // Get saved jobs
        // -----------------------------------------------------

        var savedJobs =
            await _savedJobRepository
                .GetByCandidateIdAsync(candidate.Id);


        // -----------------------------------------------------
        // Convert to response
        // -----------------------------------------------------

        return savedJobs
            .Select(MapToResponse)
            .ToList();
    }


    // =========================================================
    // CHECK WHETHER JOB IS SAVED
    // =========================================================

    public async Task<bool> IsSavedAsync(
        Guid userId,
        Guid jobId)
    {
        // -----------------------------------------------------
        // Get candidate
        // -----------------------------------------------------

        var candidate =
            await _candidateRepository.GetByUserIdAsync(userId);

        if (candidate is null)
        {
            throw new KeyNotFoundException(
                "Candidate profile not found.");
        }


        // -----------------------------------------------------
        // Check saved job
        // -----------------------------------------------------

        var savedJob =
            await _savedJobRepository.GetAsync(
                candidate.Id,
                jobId);

        return savedJob is not null;
    }


    // =========================================================
    // REMOVE SAVED JOB
    // =========================================================

    public async Task<bool> RemoveAsync(
        Guid userId,
        Guid jobId)
    {
        // -----------------------------------------------------
        // Get candidate
        // -----------------------------------------------------

        var candidate =
            await _candidateRepository.GetByUserIdAsync(userId);

        if (candidate is null)
        {
            throw new KeyNotFoundException(
                "Candidate profile not found.");
        }


        // -----------------------------------------------------
        // Find saved job
        // -----------------------------------------------------

        var savedJob =
            await _savedJobRepository.GetAsync(
                candidate.Id,
                jobId);

        if (savedJob is null)
        {
            return false;
        }


        // -----------------------------------------------------
        // Delete
        // -----------------------------------------------------

        await _savedJobRepository.DeleteAsync(
            savedJob);

        return true;
    }


    // =========================================================
    // MAP ENTITY → DTO
    // =========================================================

    private static SavedJobResponse MapToResponse(
        SavedJob savedJob)
    {
        var job = savedJob.Job;

        return new SavedJobResponse
        {
            Id = savedJob.Id,

            JobId = savedJob.JobId,

            JobTitle = job.Title,

            Location = job.Location,

            EmploymentType = job.EmploymentType,

            MinimumExperience =
                job.MinimumExperience,

            MaximumExperience =
                job.MaximumExperience,

            MinimumSalary =
                job.MinimumSalary,

            MaximumSalary =
                job.MaximumSalary,

            ApplicationDeadline =
                job.ApplicationDeadline,

            SavedAt =
                savedJob.SavedAt
        };
    }
}