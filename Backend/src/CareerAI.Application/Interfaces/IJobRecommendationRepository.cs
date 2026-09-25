using CareerAI.Domain.Entities;

namespace CareerAI.Application.Interfaces;

public interface IJobRecommendationRepository
{
    Task<List<JobRecommendation>> GetByCandidateIdAsync(
        Guid candidateId);

    Task<JobRecommendation?> GetByCandidateAndJobAsync(
        Guid candidateId,
        Guid jobId);

    Task AddAsync(JobRecommendation recommendation);

    void Update(JobRecommendation recommendation);

    Task SaveChangesAsync();
}