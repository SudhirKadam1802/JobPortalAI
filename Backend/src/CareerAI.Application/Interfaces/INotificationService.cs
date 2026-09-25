using CareerAI.Application.Features.Notifications.DTOs;

namespace CareerAI.Application.Interfaces;

public interface INotificationService
{
    Task<List<NotificationResponse>> GetMyNotificationsAsync(
        Guid userId);

    Task<NotificationResponse?> GetByIdAsync(
        Guid userId,
        Guid notificationId);

    Task<NotificationResponse> CreateAsync(
        CreateNotificationRequest request);

    Task<bool> MarkAsReadAsync(
        Guid userId,
        Guid notificationId);
}