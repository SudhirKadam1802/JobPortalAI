using Microsoft.AspNetCore.Http;

namespace CareerAI.Application.Features.Resumes.DTOs;

public class CreateResumeRequest
{
    public IFormFile File { get; set; } = null!;
}