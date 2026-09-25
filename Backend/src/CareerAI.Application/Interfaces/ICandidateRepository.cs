using CareerAI.Domain.Entities;

namespace CareerAI.Application.Interfaces;

public interface ICandidateRepository
{
    Task<Candidate?> GetByUserIdAsync(Guid userId);

    Task AddAsync(Candidate candidate);

    Task SaveChangesAsync();
}