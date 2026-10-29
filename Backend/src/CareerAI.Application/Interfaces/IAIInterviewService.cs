using CareerAI.Application.Features.AIInterview.DTOs;

namespace CareerAI.Application.Interfaces;

public interface IAIInterviewService
{
    Task<StartInterviewResponse> StartInterviewAsync(
        Guid userId,
        StartInterviewRequest request,
        CancellationToken cancellationToken = default);

    Task<SubmitAnswerResponse> SubmitAnswerAsync(
        Guid userId,
        Guid interviewId,
        SubmitAnswerRequest request,
        CancellationToken cancellationToken = default);

    Task<InterviewResultResponse?> GetInterviewResultAsync(
        Guid userId,
        Guid interviewId);
}