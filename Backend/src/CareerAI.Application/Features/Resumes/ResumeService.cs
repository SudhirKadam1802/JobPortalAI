using CareerAI.Application.Features.Resumes.DTOs;
using CareerAI.Application.Interfaces;
using CareerAI.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace CareerAI.Application.Features.Resumes;

public class ResumeService : IResumeService
{
    private readonly IResumeRepository _resumeRepository;
    private readonly ICandidateRepository _candidateRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IResumeTextExtractor _resumeTextExtractor;

    public ResumeService(
       IResumeRepository resumeRepository,
       ICandidateRepository candidateRepository,
       IFileStorageService fileStorageService,
       IResumeTextExtractor resumeTextExtractor)
    {
        _resumeRepository = resumeRepository;
        _candidateRepository = candidateRepository;
        _fileStorageService = fileStorageService;
        _resumeTextExtractor = resumeTextExtractor;
    }

    public async Task<ResumeResponse> UploadAsync(
        Guid userId,
        IFormFile file)
    {
        // 1. Validate file
        if (file is null || file.Length == 0)
        {
            throw new Exception("Resume file is required.");
        }

        // 2. Find candidate profile
        var candidate = await _candidateRepository
            .GetByUserIdAsync(userId);

        if (candidate is null)
        {
            throw new UnauthorizedAccessException(
                "Candidate profile was not found.");
        }

        // 3. Save physical file
        var filePath = await _fileStorageService
            .SaveFileAsync(file);

        // 4. Create database record
        var resume = new Resume
        {
            Id = Guid.NewGuid(),
            CandidateId = candidate.Id,
            FileName = file.FileName,
            FilePath = filePath,
            FileType = Path.GetExtension(file.FileName)
                .ToLowerInvariant(),
            UploadedAt = DateTime.UtcNow
        };

        try
        {
            await _resumeRepository.AddAsync(resume);
            await _resumeRepository.SaveChangesAsync();
        }
        catch
        {
            // If database save fails, remove the physical file
            await _fileStorageService
                .DeleteFileAsync(filePath);

            throw;
        }

        return new ResumeResponse
        {
            Id = resume.Id,
            CandidateId = resume.CandidateId,
            FileName = resume.FileName,
            FileType = resume.FileType,
            UploadedAt = resume.UploadedAt
        };
    }

    public async Task<List<ResumeResponse>> GetMyResumesAsync(
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

        var resumes = await _resumeRepository
            .GetByCandidateIdAsync(candidate.Id);

        return resumes
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<ResumeResponse?> GetByIdAsync(
        Guid userId,
        Guid resumeId)
    {
        var resume = await _resumeRepository
            .GetByIdAsync(resumeId);

        if (resume is null)
        {
            return null;
        }

        // Find candidate
        var candidate = await _candidateRepository
            .GetByUserIdAsync(userId);

        if (candidate is null)
        {
            throw new UnauthorizedAccessException(
                "Candidate profile was not found.");
        }

        // Make sure resume belongs to candidate
        if (resume.CandidateId != candidate.Id)
        {
            throw new UnauthorizedAccessException(
                "You are not authorized to access this resume.");
        }

        return MapToResponse(resume);
    }

    public async Task<string> ExtractTextAsync(
    Guid userId,
    Guid resumeId)
    {
        // 1. Find candidate
        var candidate = await _candidateRepository
            .GetByUserIdAsync(userId);

        if (candidate is null)
        {
            throw new UnauthorizedAccessException(
                "Candidate profile was not found.");
        }

        // 2. Find resume
        var resume = await _resumeRepository
            .GetByIdAsync(resumeId);

        if (resume is null)
        {
            throw new Exception("Resume not found.");
        }

        // 3. Make sure resume belongs to candidate
        if (resume.CandidateId != candidate.Id)
        {
            throw new UnauthorizedAccessException(
                "You are not authorized to access this resume.");
        }

        // 4. Extract text
        var text = await _resumeTextExtractor
            .ExtractTextAsync(resume.FilePath);

        if (string.IsNullOrWhiteSpace(text))
        {
            throw new Exception(
                "No readable text was found in the resume.");
        }

        return text;
    }
    private static ResumeResponse MapToResponse(
        Resume resume)
    {
        return new ResumeResponse
        {
            Id = resume.Id,
            CandidateId = resume.CandidateId,
            FileName = resume.FileName,
            FileType = resume.FileType,
            UploadedAt = resume.UploadedAt
        };
    }
}