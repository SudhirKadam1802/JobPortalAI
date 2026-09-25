using CareerAI.Domain.Entities;

namespace CareerAI.Application.Interfaces;

public interface IAIInterviewRepository
{
    Task<Interview?> GetInterviewAsync(
        Guid userId,
        Guid interviewId);

    Task<InterviewQuestion?> GetQuestionAsync(
        Guid interviewId,
        Guid questionId);

    Task<List<InterviewQuestion>> GetQuestionsAsync(
        Guid interviewId);

    Task AddInterviewAsync(
        Interview interview);

    Task AddQuestionAsync(
        InterviewQuestion question);

    Task AddEvaluationAsync(
        InterviewEvaluation evaluation);

    Task SaveChangesAsync();
}