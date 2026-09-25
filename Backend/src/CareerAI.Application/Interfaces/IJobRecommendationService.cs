using CareerAI.Application.Features.JobRecommendations.DTOs;

namespace CareerAI.Application.Interfaces;

public interface IJobRecommendationService
{
    Task<List<JobRecommendationResponse>> GenerateRecommendationsAsync(
        Guid userId);

    Task<List<JobRecommendationResponse>> GetMyRecommendationsAsync(
        Guid userId);
}