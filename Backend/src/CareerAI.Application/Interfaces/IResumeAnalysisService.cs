using CareerAI.Application.Features.Resumes.Analysis;

namespace CareerAI.Application.Interfaces;

public interface IResumeAnalysisService
{
    Task<ResumeAnalysisResponse> AnalyzeAsync(
        Guid userId,
        Guid resumeId);

    Task<ResumeAnalysisResponse?> GetAnalysisAsync(
        Guid userId,
        Guid resumeId);
}