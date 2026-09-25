using CareerAI.Application.Features.Resumes.DTOs;
using Microsoft.AspNetCore.Http;

namespace CareerAI.Application.Interfaces;

public interface IResumeService
{
    Task<ResumeResponse> UploadAsync(
        Guid userId,
        IFormFile file);

    Task<List<ResumeResponse>> GetMyResumesAsync(
        Guid userId);

    Task<ResumeResponse?> GetByIdAsync(
        Guid userId,
        Guid resumeId);

    Task<string> ExtractTextAsync(
        Guid userId,
        Guid resumeId);
}