using CareerAI.Application.Interfaces;
using CareerAI.Domain.Entities;
using CareerAI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CareerAI.Infrastructure.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly CareerAIDbContext _context;

    public NotificationRepository(
        CareerAIDbContext context)
    {
        _context = context;
    }

    public async Task<List<Notification>> GetByUserIdAsync(
        Guid userId)
    {
        return await _context.Notifications
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<Notification?> GetByIdAsync(
        Guid userId,
        Guid notificationId)
    {
        return await _context.Notifications
            .FirstOrDefaultAsync(
                x =>
                    x.Id == notificationId &&
                    x.UserId == userId);
    }

    public async Task AddAsync(
        Notification notification)
    {
        await _context.Notifications.AddAsync(
            notification);

        await _context.SaveChangesAsync();
    }

    public async Task MarkAsReadAsync(
        Notification notification)
    {
        notification.IsRead = true;

        await _context.SaveChangesAsync();
    }
}