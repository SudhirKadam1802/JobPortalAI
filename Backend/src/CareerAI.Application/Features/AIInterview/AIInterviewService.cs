using CareerAI.Application.Features.AIInterview.DTOs;
using CareerAI.Application.Interfaces;
using CareerAI.Domain.Entities;

namespace CareerAI.Application.Features.AIInterview;

public class AIInterviewService : IAIInterviewService
{
    private readonly IAIInterviewRepository _interviewRepository;
    private readonly IOllamaService _ollamaService;
    private readonly ICandidateRepository _candidateRepository;

    public AIInterviewService(
      IAIInterviewRepository interviewRepository,
      IOllamaService ollamaService,
      ICandidateRepository candidateRepository)
    {
        _interviewRepository = interviewRepository;
        _ollamaService = ollamaService;
        _candidateRepository = candidateRepository;
    }

    public async Task<StartInterviewResponse> StartInterviewAsync(
        Guid userId,
        StartInterviewRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.JobTitle))
        {
            throw new ArgumentException(
                "Job title cannot be empty.");
        }

        if (request.NumberOfQuestions < 1 ||
            request.NumberOfQuestions > 10)
        {
            throw new ArgumentException(
                "Number of questions must be between 1 and 10.");
        }
        var candidate =
    await _candidateRepository
        .GetByUserIdAsync(userId);

        if (candidate is null)
        {
            throw new KeyNotFoundException(
                "Candidate profile not found.");
        }

        var interview = new Interview
        {
            Id = Guid.NewGuid(),
            CandidateId = candidate.Id,
            InterviewType = request.JobTitle.Trim(),
            Status = "InProgress",
            StartedAt = DateTime.UtcNow
        };

        await _interviewRepository
            .AddInterviewAsync(interview);

        var prompt = BuildQuestionGenerationPrompt(
            request.JobTitle.Trim(),
            request.NumberOfQuestions);

        var aiResponse =
            await _ollamaService.GenerateAsync(prompt);

        var questions =
            ParseQuestions(aiResponse);

        if (questions.Count == 0)
        {
            throw new Exception(
                "AI did not generate any interview questions.");
        }

        for (int i = 0; i < questions.Count; i++)
        {
            var question = new InterviewQuestion
            {
                Id = Guid.NewGuid(),
                InterviewId = interview.Id,
                Question = questions[i],
                Order = i + 1
            };

            await _interviewRepository
                .AddQuestionAsync(question);
        }

        await _interviewRepository
            .SaveChangesAsync();

        var savedQuestions =
            await _interviewRepository
                .GetQuestionsAsync(interview.Id);

        var firstQuestion = savedQuestions
            .OrderBy(x => x.Order)
            .First();

        return new StartInterviewResponse
        {
            InterviewId = interview.Id,
            QuestionId = firstQuestion.Id,
            Question = firstQuestion.Question,
            QuestionNumber = firstQuestion.Order,
            TotalQuestions = savedQuestions.Count
        };
    }

    public async Task<SubmitAnswerResponse> SubmitAnswerAsync(
        Guid userId,
        Guid interviewId,
        SubmitAnswerRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Answer))
        {
            throw new ArgumentException(
                "Answer cannot be empty.");
        }

        var interview =
            await _interviewRepository
                .GetInterviewAsync(
                    userId,
                    interviewId);

        if (interview is null)
        {
            throw new KeyNotFoundException(
                "Interview not found.");
        }

        var question =
            await _interviewRepository
                .GetQuestionAsync(
                    interviewId,
                    request.QuestionId);

        if (question is null)
        {
            throw new KeyNotFoundException(
                "Interview question not found.");
        }

        var evaluationPrompt =
            BuildEvaluationPrompt(
                question.Question,
                request.Answer.Trim());

        var aiResponse =
            await _ollamaService
                .GenerateAsync(evaluationPrompt);

        var evaluation =
            ParseEvaluation(aiResponse);

        var interviewEvaluation =
            new InterviewEvaluation
            {
                Id = Guid.NewGuid(),
                InterviewQuestionId = question.Id,
                Score = evaluation.Score,
                Feedback = evaluation.Feedback
            };

        await _interviewRepository
            .AddEvaluationAsync(
                interviewEvaluation);

        var questions =
            await _interviewRepository
                .GetQuestionsAsync(interviewId);

        var currentOrder = question.Order;

        var nextQuestion = questions
            .Where(x => x.Order > currentOrder)
            .OrderBy(x => x.Order)
            .FirstOrDefault();

        if (nextQuestion is null)
        {
            interview.Status = "Completed";
            interview.CompletedAt = DateTime.UtcNow;

            await _interviewRepository
                .SaveChangesAsync();

            return new SubmitAnswerResponse
            {
                QuestionId = question.Id,
                Score = evaluation.Score,
                Feedback = evaluation.Feedback,
                IsInterviewCompleted = true,
                NextQuestionId = null,
                NextQuestion = null,
                NextQuestionNumber = null,
                TotalQuestions = questions.Count
            };
        }

        await _interviewRepository
            .SaveChangesAsync();

        return new SubmitAnswerResponse
        {
            QuestionId = question.Id,
            Score = evaluation.Score,
            Feedback = evaluation.Feedback,
            IsInterviewCompleted = false,
            NextQuestionId = nextQuestion.Id,
            NextQuestion = nextQuestion.Question,
            NextQuestionNumber = nextQuestion.Order,
            TotalQuestions = questions.Count
        };
    }

    public async Task<InterviewResultResponse?>
        GetInterviewResultAsync(
            Guid userId,
            Guid interviewId)
    {
        var interview =
            await _interviewRepository
                .GetInterviewAsync(
                    userId,
                    interviewId);

        if (interview is null)
        {
            return null;
        }

        var questions =
            await _interviewRepository
                .GetQuestionsAsync(
                    interviewId);

        var evaluatedQuestions =
            questions
                .Where(x => x.Evaluation != null)
                .ToList();

        if (evaluatedQuestions.Count == 0)
        {
            return new InterviewResultResponse
            {
                InterviewId = interviewId,
                TotalScore = 0,
                MaximumScore = questions.Count * 10,
                Percentage = 0,
                Strengths = "No answers have been evaluated yet.",
                Improvements = "Complete the interview to receive feedback."
            };
        }

        var totalScore =
            evaluatedQuestions
                .Sum(x => x.Evaluation!.Score);

        var maximumScore =
            evaluatedQuestions.Count * 10;

        var percentage =
            maximumScore == 0
                ? 0
                : Math.Round(
                    totalScore * 100m / maximumScore,
                    2);

        var strengths =
            evaluatedQuestions
                .Where(x => x.Evaluation!.Score >= 7)
                .Select(x => x.Evaluation!.Feedback)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x!.Trim())
                .ToList();

        var improvements =
            evaluatedQuestions
                .Where(x => x.Evaluation!.Score < 7)
                .Select(x => x.Evaluation!.Feedback)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x!.Trim())
                .ToList();

        return new InterviewResultResponse
        {
            InterviewId = interviewId,
            TotalScore = totalScore,
            MaximumScore = maximumScore,
            Percentage = percentage,
            Strengths =
                strengths.Count == 0
                    ? "No major strengths identified yet."
                    : string.Join(
                        " | ",
                        strengths),

            Improvements =
                improvements.Count == 0
                    ? "No major improvement areas identified."
                    : string.Join(
                        " | ",
                        improvements)
        };
    }

    private static string BuildQuestionGenerationPrompt(
        string jobTitle,
        int numberOfQuestions)
    {
        return $$"""
        You are CareerAI, an AI technical interviewer.

        Generate {{numberOfQuestions}} interview questions
        for the following job role:

        Job Role:
        {{jobTitle}}

        Rules:
        - Focus on technical interview questions.
        - Questions should be suitable for a candidate preparing
          for this role.
        - Mix fundamental and practical questions.
        - Do not provide answers.
        - Return only the questions.
        - Put each question on a separate line.
        - Number the questions from 1 to {{numberOfQuestions}}.

        Example format:

        1. What is dependency injection in .NET?
        2. What is middleware in ASP.NET Core?
        3. What is Entity Framework Core?
        """;
    }

    private static string BuildEvaluationPrompt(
        string question,
        string answer)
    {
        return $$"""
        You are CareerAI, an AI technical interview evaluator.

        Evaluate the candidate's answer.

        Question:
        {{question}}

        Candidate Answer:
        {{answer}}

        Give a score from 0 to 10.

        Evaluate based on:
        - Technical correctness
        - Understanding
        - Completeness
        - Clarity

        Return exactly this format:

        Score: <number>
        Feedback: <short professional feedback>

        Do not use markdown.
        Do not invent information.
        """;
    }

    private static List<string> ParseQuestions(
        string aiResponse)
    {
        if (string.IsNullOrWhiteSpace(aiResponse))
        {
            return new List<string>();
        }

        return aiResponse
            .Split(
                '\n',
                StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(RemoveQuestionNumber)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToList();
    }

    private static string RemoveQuestionNumber(
        string question)
    {
        var trimmed = question.Trim();

        var dotIndex = trimmed.IndexOf('.');

        if (dotIndex > 0 &&
            int.TryParse(
                trimmed[..dotIndex],
                out _))
        {
            return trimmed[(dotIndex + 1)..].Trim();
        }

        return trimmed;
    }

    private static (int Score, string Feedback)
        ParseEvaluation(string aiResponse)
    {
        if (string.IsNullOrWhiteSpace(aiResponse))
        {
            throw new Exception(
                "AI returned an empty evaluation.");
        }

        var lines =
            aiResponse
                .Split(
                    '\n',
                    StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .ToList();

        var scoreLine =
            lines.FirstOrDefault(
                x => x.StartsWith(
                    "Score:",
                    StringComparison.OrdinalIgnoreCase));

        var feedbackLine =
            lines.FirstOrDefault(
                x => x.StartsWith(
                    "Feedback:",
                    StringComparison.OrdinalIgnoreCase));

        if (scoreLine is null ||
            feedbackLine is null)
        {
            throw new Exception(
                "AI returned an invalid evaluation.");
        }

        var scoreText =
            scoreLine["Score:".Length..].Trim();

        if (!int.TryParse(
                scoreText,
                out var score))
        {
            throw new Exception(
                "AI returned an invalid score.");
        }

        score = Math.Clamp(score, 0, 10);

        var feedback =
            feedbackLine["Feedback:".Length..].Trim();

        if (string.IsNullOrWhiteSpace(feedback))
        {
            feedback = "No feedback provided.";
        }

        return (score, feedback);
    }
}