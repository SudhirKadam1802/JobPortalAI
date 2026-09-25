using CareerAI.Application.Features.AIInterview.DTOs;

namespace CareerAI.Application.Interfaces;

public interface IAIInterviewService
{
    Task<StartInterviewResponse> StartInterviewAsync(
        Guid userId,
        StartInterviewRequest request);

    Task<SubmitAnswerResponse> SubmitAnswerAsync(
        Guid userId,
        Guid interviewId,
        SubmitAnswerRequest request);

    Task<InterviewResultResponse?> GetInterviewResultAsync(
        Guid userId,
        Guid interviewId);
}