using CareerAI.Domain.Entities;

namespace CareerAI.Application.Interfaces;

public interface IJobAlertRepository
{
    Task<JobAlert?> GetByIdAsync(
        Guid id);

    Task<List<JobAlert>> GetByCandidateIdAsync(
        Guid candidateId);

    Task AddAsync(
        JobAlert jobAlert);

    Task UpdateAsync(
        JobAlert jobAlert);

    Task DeleteAsync(
        JobAlert jobAlert);
}