using CareerAI.Domain.Entities;

namespace CareerAI.Application.Interfaces;

public interface IResumeAnalysisRepository
{
    Task<ResumeAnalysis?> GetByResumeIdAsync(Guid resumeId);

    Task AddAsync(ResumeAnalysis analysis);

    Task SaveChangesAsync();
}