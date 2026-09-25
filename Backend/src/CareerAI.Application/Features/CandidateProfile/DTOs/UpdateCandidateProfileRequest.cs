namespace CareerAI.Application.Features.CandidateProfile.DTOs;

public class UpdateCandidateProfileRequest
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string? PhoneNumber { get; set; }

    public string? Location { get; set; }

    public string? Bio { get; set; }

    public DateTime? DateOfBirth { get; set; }
}