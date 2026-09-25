using CareerAI.Application.Features.CandidateProfile.DTOs;

namespace CareerAI.Application.Interfaces;

public interface ICandidateProfileService
{
    Task<CandidateProfileResponse?> GetMyProfileAsync(
        Guid userId);

    Task<CandidateProfileResponse?> UpdateMyProfileAsync(
        Guid userId,
        UpdateCandidateProfileRequest request);
}