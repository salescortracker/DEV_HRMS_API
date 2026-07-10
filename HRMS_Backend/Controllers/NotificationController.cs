using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {

        private readonly INotificationService _notificationService;


        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }



        // Get user notifications
        [HttpGet("GetByUser/{userId}")]
        public async Task<IActionResult> GetByUser(int userId)
        {
            try
            {
                var notifications =
                    await _notificationService.GetUserNotificationsAsync(userId);

                return Ok(notifications);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }



        // Mark notification as read
        [HttpPut("MarkAsRead/{notificationId}")]
        public async Task<IActionResult> MarkAsRead(int notificationId)
        {
            try
            {
                await _notificationService.MarkAsReadAsync(notificationId);

                return Ok(new
                {
                    message = "Notification marked as read"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }



        // Testing API (temporary)
        [HttpPost("Create")]
        public async Task<IActionResult> Create(NotificationCreateDto dto)
        {
            try
            {

                await _notificationService.CreateNotificationAsync(
                    dto.UserIds,
                    dto.Title,
                    dto.Message,
                    dto.Type,
                    dto.ReferenceId
                );


                return Ok(new
                {
                    message = "Notification created successfully"
                });

            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

    }
}