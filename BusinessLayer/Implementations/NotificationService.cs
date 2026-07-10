using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using Microsoft.EntityFrameworkCore;

namespace BusinessLayer.Implementations
{
    public class NotificationService : INotificationService
    {

        private readonly HRMSContext _context;


        public NotificationService(HRMSContext context)
        {
            _context = context;
        }



        public async Task CreateNotificationAsync(
        List<int> userIds,
        string title,
        string message,
        string type,
        int? referenceId = null)
        {

            var notifications = userIds
            .Distinct()
            .Select(id => new Notification
            {
                UserId = id,
                Title = title,
                Message = message,
                Type = type,
                ReferenceId = referenceId,
                IsRead = false,
                CreatedDate = DateTime.Now
            })
            .ToList();


            await _context.Notifications.AddRangeAsync(notifications);

            await _context.SaveChangesAsync();

        }
        public async Task<List<NotificationDto>> GetUserNotificationsAsync(int userId)
        {

            return await _context.Notifications
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedDate)
                .Select(x => new NotificationDto
                {
                    NotificationId = x.NotificationId,
                    UserId = x.UserId,
                    Title = x.Title,
                    Message = x.Message,
                    Type = x.Type,
                    ReferenceId = x.ReferenceId,
                    IsRead = x.IsRead,
                    CreatedDate = x.CreatedDate
                })
                .ToListAsync();

        }



        public async Task MarkAsReadAsync(int notificationId)
        {

            var notification = await _context.Notifications
                .FirstOrDefaultAsync(x => x.NotificationId == notificationId);


            if (notification != null)
            {
                notification.IsRead = true;

                await _context.SaveChangesAsync();
            }

        }

    }
}
