using CareerAI.Domain.Entities;

namespace CareerAI.Application.Interfaces;

public interface INotificationRepository
{
    Task<List<Notification>> GetByUserIdAsync(Guid userId);

    Task<Notification?> GetByIdAsync(
        Guid userId,
        Guid notificationId);

    Task AddAsync(Notification notification);

    Task MarkAsReadAsync(Notification notification);
}