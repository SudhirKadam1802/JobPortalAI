using System.Text.Json;
using CareerAI.Application.Interfaces;
using CareerAI.Domain.Entities;

namespace CareerAI.Application.Features.Resumes.Analysis;

public class ResumeAnalysisService : IResumeAnalysisService
{
    private readonly IResumeRepository _resumeRepository;
    private readonly IResumeAnalysisRepository _analysisRepository;
    private readonly ICandidateRepository _candidateRepository;
    private readonly IResumeTextExtractor _resumeTextExtractor;
    private readonly IOllamaService _ollamaService;

    public ResumeAnalysisService(
        IResumeRepository resumeRepository,
        IResumeAnalysisRepository analysisRepository,
        ICandidateRepository candidateRepository,
        IResumeTextExtractor resumeTextExtractor,
        IOllamaService ollamaService)
    {
        _resumeRepository = resumeRepository;
        _analysisRepository = analysisRepository;
        _candidateRepository = candidateRepository;
        _resumeTextExtractor = resumeTextExtractor;
        _ollamaService = ollamaService;
    }

    public async Task<ResumeAnalysisResponse> AnalyzeAsync(
        Guid userId,
        Guid resumeId)
    {
        // 1. Find candidate
        var candidate = await _candidateRepository
            .GetByUserIdAsync(userId);

        if (candidate is null)
        {
            throw new UnauthorizedAccessException(
                "Candidate profile was not found.");
        }

        // 2. Find resume
        var resume = await _resumeRepository
            .GetByIdAsync(resumeId);

        if (resume is null)
        {
            throw new Exception("Resume not found.");
        }

        // 3. Verify ownership
        if (resume.CandidateId != candidate.Id)
        {
            throw new UnauthorizedAccessException(
                "You are not authorized to analyze this resume.");
        }

        // 4. Extract resume text
        var resumeText = await _resumeTextExtractor
            .ExtractTextAsync(resume.FilePath);

        if (string.IsNullOrWhiteSpace(resumeText))
        {
            throw new Exception(
                "No readable text was found in the resume.");
        }

        // 5. Create AI prompt
        var prompt = BuildPrompt(resumeText);

        // 6. Send resume to Ollama
        var aiResponse = await _ollamaService
            .GenerateAsync(prompt);

        // 7. Parse AI response
        var analysisData = ParseAIResponse(aiResponse);

        // 8. Check existing analysis
        var existingAnalysis = await _analysisRepository
            .GetByResumeIdAsync(resumeId);

        ResumeAnalysis analysis;

        if (existingAnalysis is not null)
        {
            analysis = existingAnalysis;

            analysis.Score = analysisData.Score;
            analysis.Summary = analysisData.Summary;
            analysis.ExtractedSkills = JsonSerializer.Serialize(
                analysisData.Skills);
            analysis.ExtractedEducation = analysisData.Education;
            analysis.ExtractedExperience = analysisData.Experience;
            analysis.Suggestions = JsonSerializer.Serialize(
                analysisData.Suggestions);
            analysis.AnalyzedAt = DateTime.UtcNow;
        }
        else
        {
            analysis = new ResumeAnalysis
            {
                Id = Guid.NewGuid(),
                ResumeId = resumeId,
                Score = analysisData.Score,
                Summary = analysisData.Summary,
                ExtractedSkills = JsonSerializer.Serialize(
                    analysisData.Skills),
                ExtractedEducation = analysisData.Education,
                ExtractedExperience = analysisData.Experience,
                Suggestions = JsonSerializer.Serialize(
                    analysisData.Suggestions),
                AnalyzedAt = DateTime.UtcNow
            };

            await _analysisRepository.AddAsync(analysis);
        }

        // 9. Save to SQL Server
        await _analysisRepository.SaveChangesAsync();

        // 10. Return response
        return MapToResponse(analysis, analysisData);
    }

    public async Task<ResumeAnalysisResponse?> GetAnalysisAsync(
        Guid userId,
        Guid resumeId)
    {
        var candidate = await _candidateRepository
            .GetByUserIdAsync(userId);

        if (candidate is null)
        {
            throw new UnauthorizedAccessException(
                "Candidate profile was not found.");
        }

        var resume = await _resumeRepository
            .GetByIdAsync(resumeId);

        if (resume is null)
        {
            throw new Exception("Resume not found.");
        }

        if (resume.CandidateId != candidate.Id)
        {
            throw new UnauthorizedAccessException(
                "You are not authorized to access this resume.");
        }

        var analysis = await _analysisRepository
            .GetByResumeIdAsync(resumeId);

        if (analysis is null)
        {
            return null;
        }

        var skills = DeserializeList(
            analysis.ExtractedSkills);

        var suggestions = DeserializeList(
            analysis.Suggestions);

        return new ResumeAnalysisResponse
        {
            Id = analysis.Id,
            ResumeId = analysis.ResumeId,
            Score = analysis.Score,
            Summary = analysis.Summary ?? string.Empty,
            Skills = skills,
            Education = analysis.ExtractedEducation ?? string.Empty,
            Experience = analysis.ExtractedExperience ?? string.Empty,
            Suggestions = suggestions,
            AnalyzedAt = analysis.AnalyzedAt
        };
    }

    private static string BuildPrompt(string resumeText)
    {
        return $$"""
    You are a resume analysis engine.

    Analyze the resume and return ONLY valid JSON.

    Do NOT explain your reasoning.
    Do NOT use markdown.
    Do NOT use ```json.
    Do NOT invent information.

    Return exactly this structure:

    {
      "score": 0,
      "summary": "Short summary",
      "skills": [
        "skill1",
        "skill2"
      ],
      "education": "Education information",
      "experience": "Experience information",
      "suggestions": [
        "suggestion1",
        "suggestion2",
        "suggestion3"
      ]
    }

    Rules:

    - score must be between 0 and 100
    - extract skills explicitly mentioned in the resume
    - summarize education
    - summarize experience
    - provide exactly 3 practical suggestions
    - do not invent information
    - if information is missing, return "Not mentioned"

    Resume:

    {{resumeText}}
    """;
    }

    private static AIAnalysisData ParseAIResponse(
        string aiResponse)
    {
        var cleanedResponse = aiResponse.Trim();

        if (cleanedResponse.StartsWith("```"))
        {
            cleanedResponse = cleanedResponse
                .Replace("```json", "")
                .Replace("```", "")
                .Trim();
        }

        try
        {
            var result =
                JsonSerializer.Deserialize<AIAnalysisData>(
                    cleanedResponse,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

            if (result is null)
            {
                throw new Exception(
                    "AI returned an empty analysis.");
            }

            if (result.Score < 0 || result.Score > 100)
            {
                result.Score = Math.Clamp(
                    result.Score,
                    0,
                    100);
            }

            return result;
        }
        catch (JsonException)
        {
            throw new Exception(
                "AI returned an invalid analysis format.");
        }
    }

    private static ResumeAnalysisResponse MapToResponse(
        ResumeAnalysis analysis,
        AIAnalysisData data)
    {
        return new ResumeAnalysisResponse
        {
            Id = analysis.Id,
            ResumeId = analysis.ResumeId,
            Score = analysis.Score,
            Summary = analysis.Summary ?? string.Empty,
            Skills = data.Skills,
            Education = analysis.ExtractedEducation ?? string.Empty,
            Experience = analysis.ExtractedExperience ?? string.Empty,
            Suggestions = data.Suggestions,
            AnalyzedAt = analysis.AnalyzedAt
        };
    }

    private static List<string> DeserializeList(
        string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return new List<string>();
        }

        try
        {
            return JsonSerializer.Deserialize<List<string>>(json)
                   ?? new List<string>();
        }
        catch (JsonException)
        {
            return new List<string>();
        }
    }

    private class AIAnalysisData
    {
        public int Score { get; set; }

        public string Summary { get; set; } = string.Empty;

        public List<string> Skills { get; set; } = new();

        public string Education { get; set; } = string.Empty;

        public string Experience { get; set; } = string.Empty;

        public List<string> Suggestions { get; set; } = new();
    }
}