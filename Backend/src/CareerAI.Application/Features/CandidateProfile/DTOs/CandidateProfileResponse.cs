namespace CareerAI.Application.Features.CandidateProfile.DTOs;

public class CandidateProfileResponse
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? PhoneNumber { get; set; }

    public string? Location { get; set; }

    public string? Bio { get; set; }

    public DateTime? DateOfBirth { get; set; }
}