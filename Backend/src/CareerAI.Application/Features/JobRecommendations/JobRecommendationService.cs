using System.Text.Json;
using CareerAI.Application.Features.JobRecommendations.DTOs;
using CareerAI.Application.Interfaces;
using CareerAI.Domain.Entities;

namespace CareerAI.Application.Features.JobRecommendations;

public class JobRecommendationService : IJobRecommendationService
{
    private readonly ICandidateRepository _candidateRepository;
    private readonly IJobRepository _jobRepository;
    private readonly IJobRecommendationRepository _recommendationRepository;
    private readonly IOllamaService _ollamaService;

    public JobRecommendationService(
        ICandidateRepository candidateRepository,
        IJobRepository jobRepository,
        IJobRecommendationRepository recommendationRepository,
        IOllamaService ollamaService)
    {
        _candidateRepository = candidateRepository;
        _jobRepository = jobRepository;
        _recommendationRepository = recommendationRepository;
        _ollamaService = ollamaService;
    }

    // ========================================
    // Generate Job Recommendations
    // ========================================

    public async Task<List<JobRecommendationResponse>>
        GenerateRecommendationsAsync(Guid userId)
    {
        // 1. Find candidate using logged-in user's ID
        var candidate = await _candidateRepository
            .GetByUserIdAsync(userId);

        if (candidate is null)
        {
            throw new UnauthorizedAccessException(
                "Candidate profile was not found.");
        }

        // ========================================
        // 2. Get Candidate Skills
        // ========================================

        var candidateSkills = candidate.CandidateSkills
            .Select(x => x.Skill.Name)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        // ========================================
        // 3. Calculate Candidate Experience
        // ========================================

        var candidateExperienceYears =
            CalculateExperienceYears(
                candidate.Experiences.ToList());

        // ========================================
        // 4. Get All Jobs
        // ========================================

        var jobs = await _jobRepository
            .GetAllAsync();

        var recommendations =
            new List<JobRecommendationResponse>();

        // ========================================
        // 5. Compare Candidate With Every Job
        // ========================================

        foreach (var job in jobs)
        {
            // ========================================
            // Get Required Job Skills
            // ========================================

            var requiredSkills = job.JobSkills
                .Select(x => x.Skill.Name)
                .ToList();

            // Ignore jobs without required skills
            if (requiredSkills.Count == 0)
            {
                continue;
            }

            // ========================================
            // Find Matched Skills
            // ========================================

            var matchedSkills = requiredSkills
                .Where(skill =>
                    candidateSkills.Contains(skill))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            // ========================================
            // Find Missing Skills
            // ========================================

            var missingSkills = requiredSkills
                .Where(skill =>
                    !candidateSkills.Contains(skill))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            // ========================================
            // Calculate Skill Match Score
            // Weight = 60%
            // ========================================

            var skillMatchScore =
                (decimal)matchedSkills.Count
                / requiredSkills.Count
                * 100;

            // ========================================
            // Calculate Experience Match Score
            // Weight = 20%
            // ========================================

            var experienceMatchScore =
                CalculateExperienceMatchScore(
                    candidateExperienceYears,
                    job.MinimumExperience,
                    job.MaximumExperience);

            // ========================================
            // Calculate Location Match Score
            // Weight = 10%
            // ========================================

            var locationMatchScore =
                CalculateLocationMatchScore(
                    candidate.Location,
                    job.Location);

            // ========================================
            // Calculate Education Match Score
            // Weight = 10%
            // ========================================

            var educationMatchScore =
                CalculateEducationMatchScore(
                    candidate.Educations.ToList(),
                    job.RequiredEducation);

            // ========================================
            // Calculate Final Match Score
            //
            // Skills       = 60%
            // Experience   = 20%
            // Location     = 10%
            // Education    = 10%
            // ========================================

            var finalMatchScore =
                Math.Round(
                    (skillMatchScore * 0.60m)
                    + (experienceMatchScore * 0.20m)
                    + (locationMatchScore * 0.10m)
                    + (educationMatchScore * 0.10m),
                    2);

            // ========================================
            // Generate AI Explanation
            // ========================================

            var aiExplanation =
                await GenerateAIExplanationAsync(
                    job.Title,
                    finalMatchScore,
                    candidateExperienceYears,
                    job.MinimumExperience,
                    job.MaximumExperience,
                    candidate.Location,
                    job.Location,
                    job.RequiredEducation,
                    educationMatchScore,
                    matchedSkills,
                    missingSkills);

            // ========================================
            // Check Existing Recommendation
            // ========================================

            var existingRecommendation =
                await _recommendationRepository
                    .GetByCandidateAndJobAsync(
                        candidate.Id,
                        job.Id);

            JobRecommendation recommendation;

            // ========================================
            // Update Existing Recommendation
            // ========================================

            if (existingRecommendation is not null)
            {
                recommendation =
                    existingRecommendation;

                recommendation.MatchScore =
                    finalMatchScore;

                recommendation.MatchedSkills =
                    JsonSerializer.Serialize(
                        matchedSkills);

                recommendation.MissingSkills =
                    JsonSerializer.Serialize(
                        missingSkills);

                recommendation.Explanation =
                    aiExplanation;

                recommendation.CreatedAt =
                    DateTime.UtcNow;

                _recommendationRepository.Update(
                    recommendation);
            }

            // ========================================
            // Create New Recommendation
            // ========================================

            else
            {
                recommendation =
                    new JobRecommendation
                    {
                        Id = Guid.NewGuid(),

                        CandidateId =
                            candidate.Id,

                        JobId =
                            job.Id,

                        MatchScore =
                            finalMatchScore,

                        MatchedSkills =
                            JsonSerializer.Serialize(
                                matchedSkills),

                        MissingSkills =
                            JsonSerializer.Serialize(
                                missingSkills),

                        Explanation =
                            aiExplanation,

                        CreatedAt =
                            DateTime.UtcNow
                    };

                await _recommendationRepository
                    .AddAsync(recommendation);
            }

            // ========================================
            // Add API Response
            // ========================================

            recommendations.Add(
                MapToResponse(
                    recommendation,
                    job.Title,
                    job.Location,
                    matchedSkills,
                    missingSkills));
        }

        // ========================================
        // Save Changes
        // ========================================

        await _recommendationRepository
            .SaveChangesAsync();

        // ========================================
        // Return Highest Matches First
        // ========================================

        return recommendations
            .OrderByDescending(x => x.MatchScore)
            .ToList();
    }

    // ========================================
    // Get My Recommendations
    // ========================================

    public async Task<List<JobRecommendationResponse>>
        GetMyRecommendationsAsync(Guid userId)
    {
        // 1. Find candidate
        var candidate = await _candidateRepository
            .GetByUserIdAsync(userId);

        if (candidate is null)
        {
            throw new UnauthorizedAccessException(
                "Candidate profile was not found.");
        }

        // 2. Get saved recommendations
        var recommendations =
            await _recommendationRepository
                .GetByCandidateIdAsync(
                    candidate.Id);

        // 3. Convert entities to response DTOs
        return recommendations
            .Select(recommendation =>
            {
                var matchedSkills =
                    DeserializeList(
                        recommendation.MatchedSkills);

                var missingSkills =
                    DeserializeList(
                        recommendation.MissingSkills);

                return MapToResponse(
                    recommendation,
                    recommendation.Job.Title,
                    recommendation.Job.Location,
                    matchedSkills,
                    missingSkills);
            })
            .ToList();
    }

    // ========================================
    // Calculate Total Candidate Experience
    // ========================================

    private static decimal CalculateExperienceYears(
        List<Experience> experiences)
    {
        if (experiences.Count == 0)
        {
            return 0;
        }

        var totalDays = 0.0;

        foreach (var experience in experiences)
        {
            var startDate =
                experience.StartDate;

            var endDate =
                experience.IsCurrent
                    ? DateTime.UtcNow
                    : experience.EndDate
                        ?? DateTime.UtcNow;

            if (endDate <= startDate)
            {
                continue;
            }

            totalDays +=
                (endDate - startDate).TotalDays;
        }

        return Math.Round(
            (decimal)(totalDays / 365.25),
            2);
    }

    // ========================================
    // Calculate Experience Match Score
    // ========================================

    private static decimal CalculateExperienceMatchScore(
        decimal candidateExperienceYears,
        int minimumExperience,
        int maximumExperience)
    {
        // No experience requirement
        if (minimumExperience == 0 &&
            maximumExperience == 0)
        {
            return 100;
        }

        // Candidate has less experience
        // than the minimum requirement
        if (candidateExperienceYears < minimumExperience)
        {
            if (minimumExperience == 0)
            {
                return 100;
            }

            var score =
                candidateExperienceYears
                / minimumExperience
                * 100;

            return Math.Clamp(
                Math.Round(score, 2),
                0,
                100);
        }

        // Candidate is within required range
        if (candidateExperienceYears >= minimumExperience &&
            candidateExperienceYears <= maximumExperience)
        {
            return 100;
        }

        // Candidate has more experience
        // than maximum requirement.
        // We don't penalize the candidate.
        return 100;
    }

    // ========================================
    // Calculate Location Match Score
    // ========================================

    private static decimal CalculateLocationMatchScore(
        string? candidateLocation,
        string? jobLocation)
    {
        // If job location is not specified,
        // don't penalize the candidate.
        if (string.IsNullOrWhiteSpace(jobLocation))
        {
            return 100;
        }

        // Candidate location is missing
        if (string.IsNullOrWhiteSpace(candidateLocation))
        {
            return 0;
        }

        return string.Equals(
            candidateLocation.Trim(),
            jobLocation.Trim(),
            StringComparison.OrdinalIgnoreCase)
            ? 100
            : 0;
    }

    // ========================================
    // Calculate Education Match Score
    // ========================================

    private static decimal CalculateEducationMatchScore(
        List<Education> educations,
        string? requiredEducation)
    {
        // Job does not specify education requirement
        if (string.IsNullOrWhiteSpace(requiredEducation))
        {
            return 100;
        }

        // Candidate has no education records
        if (educations.Count == 0)
        {
            return 0;
        }

        var required =
            requiredEducation.Trim();

        foreach (var education in educations)
        {
            var degree =
                education.Degree?.Trim()
                ?? string.Empty;

            var fieldOfStudy =
                education.FieldOfStudy?.Trim()
                ?? string.Empty;

            // Match against degree
            if (degree.Contains(
                    required,
                    StringComparison.OrdinalIgnoreCase))
            {
                return 100;
            }

            // Match against field of study
            if (fieldOfStudy.Contains(
                    required,
                    StringComparison.OrdinalIgnoreCase))
            {
                return 100;
            }
        }

        return 0;
    }

    // ========================================
    // Generate AI Explanation
    // ========================================

    private async Task<string>
        GenerateAIExplanationAsync(
            string jobTitle,
            decimal matchScore,
            decimal candidateExperienceYears,
            int minimumExperience,
            int maximumExperience,
            string? candidateLocation,
            string jobLocation,
            string? requiredEducation,
            decimal educationMatchScore,
            List<string> matchedSkills,
            List<string> missingSkills)
    {
        var matched =
            matchedSkills.Count > 0
                ? string.Join(
                    ", ",
                    matchedSkills)
                : "None";

        var missing =
            missingSkills.Count > 0
                ? string.Join(
                    ", ",
                    missingSkills)
                : "None";

        var candidateLocationText =
            string.IsNullOrWhiteSpace(candidateLocation)
                ? "Not mentioned"
                : candidateLocation;

        var requiredEducationText =
            string.IsNullOrWhiteSpace(requiredEducation)
                ? "Not specified"
                : requiredEducation;

        var educationResult =
            educationMatchScore >= 100
                ? "Matches"
                : "Does not match";

        var prompt = $$"""
        You are an AI recruitment assistant.

        Analyze the candidate's suitability for the job.

        Job Title:
        {{jobTitle}}

        Final Match Score:
        {{matchScore}}%

        Candidate Experience:
        {{candidateExperienceYears}} years

        Required Experience:
        {{minimumExperience}} to {{maximumExperience}} years

        Candidate Location:
        {{candidateLocationText}}

        Job Location:
        {{jobLocation}}

        Required Education:
        {{requiredEducationText}}

        Education Match:
        {{educationResult}}

        Matched Skills:
        {{matched}}

        Missing Skills:
        {{missing}}

        Rules:
        - Write 2 to 4 professional sentences.
        - Explain why the candidate matches the job.
        - Mention relevant matching skills.
        - Mention important missing skills.
        - Mention experience suitability when relevant.
        - Mention education or location when relevant.
        - Suggest what the candidate should learn.
        - Do not invent experience.
        - Do not invent skills.
        - Do not invent education.
        - Do not use markdown.
        - Do not use bullet points.
        - Return only the explanation.
        """;

        var response =
            await _ollamaService
                .GenerateAsync(prompt);

        return string.IsNullOrWhiteSpace(response)
            ? "No AI explanation was generated."
            : response.Trim();
    }

    // ========================================
    // Map Entity To Response DTO
    // ========================================

    private static JobRecommendationResponse
        MapToResponse(
            JobRecommendation recommendation,
            string jobTitle,
            string location,
            List<string> matchedSkills,
            List<string> missingSkills)
    {
        return new JobRecommendationResponse
        {
            Id =
                recommendation.Id,

            JobId =
                recommendation.JobId,

            JobTitle =
                jobTitle,

            Location =
                location,

            MatchScore =
                recommendation.MatchScore,

            MatchedSkills =
                matchedSkills,

            MissingSkills =
                missingSkills,

            Explanation =
                recommendation.Explanation
                ?? string.Empty,

            CreatedAt =
                recommendation.CreatedAt
        };
    }

    // ========================================
    // Deserialize JSON List
    // ========================================

    private static List<string>
        DeserializeList(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return new List<string>();
        }

        try
        {
            return JsonSerializer
                .Deserialize<List<string>>(json)
                ?? new List<string>();
        }
        catch
        {
            return new List<string>();
        }
    }
}