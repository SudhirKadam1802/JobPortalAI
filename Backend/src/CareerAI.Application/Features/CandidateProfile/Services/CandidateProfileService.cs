using CareerAI.Application.Features.CandidateProfile.DTOs;
using CareerAI.Application.Interfaces;

namespace CareerAI.Application.Features.CandidateProfile.Services;

public class CandidateProfileService
    : ICandidateProfileService
{
    private readonly ICandidateProfileRepository
        _candidateProfileRepository;

    public CandidateProfileService(
        ICandidateProfileRepository candidateProfileRepository)
    {
        _candidateProfileRepository =
            candidateProfileRepository;
    }

    public async Task<CandidateProfileResponse?>
        GetMyProfileAsync(Guid userId)
    {
        var candidate =
            await _candidateProfileRepository
                .GetByUserIdAsync(userId);

        if (candidate is null)
        {
            return null;
        }

        return MapToResponse(candidate);
    }

    public async Task<CandidateProfileResponse?>
        UpdateMyProfileAsync(
            Guid userId,
            UpdateCandidateProfileRequest request)
    {
        var candidate =
            await _candidateProfileRepository
                .GetByUserIdAsync(userId);

        if (candidate is null)
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(request.FirstName))
        {
            throw new ArgumentException(
                "First name is required.");
        }

        if (string.IsNullOrWhiteSpace(request.LastName))
        {
            throw new ArgumentException(
                "Last name is required.");
        }

        candidate.User.FirstName =
            request.FirstName.Trim();

        candidate.User.LastName =
            request.LastName.Trim();

        candidate.PhoneNumber =
            string.IsNullOrWhiteSpace(request.PhoneNumber)
                ? null
                : request.PhoneNumber.Trim();

        candidate.Location =
            string.IsNullOrWhiteSpace(request.Location)
                ? null
                : request.Location.Trim();

        candidate.Bio =
            string.IsNullOrWhiteSpace(request.Bio)
                ? null
                : request.Bio.Trim();

        // Convert DateOfBirth to UTC before saving to PostgreSQL.
        candidate.DateOfBirth =
            ToUtcNullable(request.DateOfBirth);

        candidate.User.UpdatedAt =
            DateTime.UtcNow;

        await _candidateProfileRepository
            .UpdateAsync(candidate);

        return MapToResponse(candidate);
    }

    // ========================================
    // Convert DateTime? to UTC
    // ========================================

    private static DateTime? ToUtcNullable(DateTime? value)
    {
        if (!value.HasValue)
        {
            return null;
        }

        var date = value.Value;

        if (date.Kind == DateTimeKind.Utc)
        {
            return date;
        }

        if (date.Kind == DateTimeKind.Local)
        {
            return date.ToUniversalTime();
        }

        // Angular date-only values normally arrive as
        // DateTimeKind.Unspecified.
        // Treat the date as UTC midnight.
        return DateTime.SpecifyKind(
            date,
            DateTimeKind.Utc);
    }

    private static CandidateProfileResponse
        MapToResponse(
            Domain.Entities.Candidate candidate)
    {
        return new CandidateProfileResponse
        {
            Id = candidate.Id,
            UserId = candidate.UserId,

            FirstName =
                candidate.User.FirstName,

            LastName =
                candidate.User.LastName,

            Email =
                candidate.User.Email,

            PhoneNumber =
                candidate.PhoneNumber,

            Location =
                candidate.Location,

            Bio =
                candidate.Bio,

            DateOfBirth =
                candidate.DateOfBirth
        };
    }
}