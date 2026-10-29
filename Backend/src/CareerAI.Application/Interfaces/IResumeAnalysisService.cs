using CareerAI.Application.Features.Resumes.Analysis;

namespace CareerAI.Application.Interfaces;

public interface IResumeAnalysisService
{
    Task<ResumeAnalysisResponse> AnalyzeAsync(
        Guid userId,
        Guid resumeId,
        CancellationToken cancellationToken = default);

    Task<ResumeAnalysisResponse?> GetAnalysisAsync(
        Guid userId,
        Guid resumeId);
}