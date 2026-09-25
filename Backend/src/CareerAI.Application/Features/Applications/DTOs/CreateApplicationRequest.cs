namespace CareerAI.Application.Features.Applications.DTOs;

public class CreateApplicationRequest
{
    public Guid JobId { get; set; }

    public string? CoverLetter { get; set; }
}