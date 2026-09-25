using CareerAI.Application.Interfaces;
using CareerAI.Domain.Entities;
using CareerAI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CareerAI.Infrastructure.Repositories;

public class AIInterviewRepository : IAIInterviewRepository
{
    private readonly CareerAIDbContext _context;

    public AIInterviewRepository(CareerAIDbContext context)
    {
        _context = context;
    }

    public async Task<Interview?> GetInterviewAsync(
        Guid userId,
        Guid interviewId)
    {
        return await _context.Interviews
            .Include(x => x.Candidate)
            .FirstOrDefaultAsync(x =>
                x.Id == interviewId &&
                x.Candidate.UserId == userId);
    }

    public async Task<InterviewQuestion?> GetQuestionAsync(
        Guid interviewId,
        Guid questionId)
    {
        return await _context.InterviewQuestions
            .Include(x => x.Evaluation)
            .FirstOrDefaultAsync(x =>
                x.Id == questionId &&
                x.InterviewId == interviewId);
    }

    public async Task<List<InterviewQuestion>> GetQuestionsAsync(
        Guid interviewId)
    {
        return await _context.InterviewQuestions
            .Include(x => x.Evaluation)
            .Where(x => x.InterviewId == interviewId)
            .OrderBy(x => x.Order)
            .ToListAsync();
    }

    public async Task AddInterviewAsync(
        Interview interview)
    {
        await _context.Interviews.AddAsync(interview);
    }

    public async Task AddQuestionAsync(
        InterviewQuestion question)
    {
        await _context.InterviewQuestions.AddAsync(question);
    }

    public async Task AddEvaluationAsync(
        InterviewEvaluation evaluation)
    {
        await _context.InterviewEvaluations.AddAsync(evaluation);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}