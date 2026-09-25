using CareerAI.Domain.Entities;

namespace CareerAI.Application.Interfaces;

public interface IResumeRepository
{
    Task<List<Resume>> GetByCandidateIdAsync(
        Guid candidateId);

    Task<Resume?> GetByIdAsync(
        Guid id);

    Task AddAsync(
        Resume resume);

    Task SaveChangesAsync();
}