using Microsoft.AspNetCore.Mvc;
using Cars24API.Models;
using Cars24API.Services;

namespace Cars24API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly NotificationService _notificationService;

        public NotificationController(NotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpPost("register-token")]
        public async Task<IActionResult> RegisterToken(RegisterNotificationTokenRequest request)
        {
            await _notificationService.SaveTokenAsync(request);

            return Ok(new
            {
                message = "Notification token saved successfully."
            });
        }

        [HttpPost("test")]
        public async Task<IActionResult> TestNotification(
    [FromQuery] string userId)
        {
            await _notificationService.SendNotificationAsync(
                userId,
                "Cars24",
                "🎉 Congratulations! Your first push notification is working."
            );

            return Ok(new
            {
                message = "Notification sent successfully."
            });
        }
    }
}