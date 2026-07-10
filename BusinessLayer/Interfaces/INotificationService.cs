using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.DTOs;

namespace BusinessLayer.Interfaces
{
    public interface INotificationService
    {
        Task CreateNotificationAsync(
            List<int> userIds,
            string title,
            string message,
            string type,
            int? referenceId = null
        );


        Task<List<NotificationDto>> GetUserNotificationsAsync(int userId);


        Task MarkAsReadAsync(int notificationId);
    }
}
