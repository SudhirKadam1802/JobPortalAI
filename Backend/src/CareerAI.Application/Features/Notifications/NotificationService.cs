using CareerAI.Application.Features.Notifications.DTOs;
using CareerAI.Application.Interfaces;
using CareerAI.Domain.Entities;

namespace CareerAI.Application.Features.Notifications;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;

    public NotificationService(
        INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task<List<NotificationResponse>>
        GetMyNotificationsAsync(Guid userId)
    {
        var notifications =
            await _notificationRepository
                .GetByUserIdAsync(userId);

        return notifications
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<NotificationResponse?>
        GetByIdAsync(
            Guid userId,
            Guid notificationId)
    {
        var notification =
            await _notificationRepository
                .GetByIdAsync(
                    userId,
                    notificationId);

        if (notification is null)
        {
            return null;
        }

        return MapToResponse(notification);
    }

    public async Task<NotificationResponse>
        CreateAsync(
            CreateNotificationRequest request)
    {
        var notification = new Notification
        {
            Id = Guid.NewGuid(),

            UserId = request.UserId,

            Title = request.Title,

            Message = request.Message,

            IsRead = false,

            CreatedAt = DateTime.UtcNow
        };

        await _notificationRepository
            .AddAsync(notification);

        return MapToResponse(notification);
    }

    public async Task<bool>
        MarkAsReadAsync(
            Guid userId,
            Guid notificationId)
    {
        var notification =
            await _notificationRepository
                .GetByIdAsync(
                    userId,
                    notificationId);

        if (notification is null)
        {
            return false;
        }

        await _notificationRepository
            .MarkAsReadAsync(notification);

        return true;
    }

    private static NotificationResponse
        MapToResponse(Notification notification)
    {
        return new NotificationResponse
        {
            Id = notification.Id,

            Title = notification.Title,

            Message = notification.Message,

            IsRead = notification.IsRead,

            CreatedAt = notification.CreatedAt
        };
    }
}