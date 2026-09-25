using CareerAI.Domain.Entities;

namespace CareerAI.Application.Interfaces;

public interface ICandidateProfileRepository
{
    Task<Candidate?> GetByUserIdAsync(Guid userId);

    Task UpdateAsync(
        Candidate candidate);
}