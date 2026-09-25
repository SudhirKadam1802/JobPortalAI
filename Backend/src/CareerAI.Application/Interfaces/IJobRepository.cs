using CareerAI.Domain.Entities;

namespace CareerAI.Application.Interfaces;

public interface IJobRepository
{
    Task<List<Job>> GetAllAsync();

    Task<Job?> GetByIdAsync(Guid id);

    Task AddAsync(Job job);

    void Update(Job job);

    void Delete(Job job);

    Task SaveChangesAsync();
}