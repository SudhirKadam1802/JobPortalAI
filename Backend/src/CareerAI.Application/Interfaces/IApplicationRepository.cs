using ApplicationEntity = CareerAI.Domain.Entities.Application;

namespace CareerAI.Application.Interfaces;

public interface IApplicationRepository
{
    Task<List<ApplicationEntity>> GetByCandidateIdAsync(Guid candidateId);

    Task<List<ApplicationEntity>> GetByJobIdAsync(Guid jobId);

    Task<ApplicationEntity?> GetByIdAsync(Guid id);

    Task<bool> ExistsAsync(Guid candidateId, Guid jobId);

    Task AddAsync(ApplicationEntity application);

    void Update(ApplicationEntity application);

    Task SaveChangesAsync();
}