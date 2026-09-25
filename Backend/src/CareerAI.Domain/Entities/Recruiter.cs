namespace CareerAI.Domain.Entities;

public class Recruiter
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string CompanyName { get; set; } = string.Empty;

    public string? CompanyDescription { get; set; }

    public string? CompanyWebsite { get; set; }

    public string? Location { get; set; }

    public User User { get; set; } = null!;
}