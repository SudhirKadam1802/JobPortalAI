using CareerAI.Domain.Entities;

namespace CareerAI.Application.Interfaces;

public interface IRecruiterRepository
{
    Task<Recruiter?> GetByUserIdAsync(Guid userId);

    Task<Recruiter?> GetByIdAsync(Guid id);

    Task AddAsync(Recruiter recruiter);

    Task SaveChangesAsync();
}