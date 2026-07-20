namespace Cars24API.Models
{
    public class RegisterNotificationTokenRequest
    {
        public string UserId { get; set; } = string.Empty;

        public string Token { get; set; } = string.Empty;
    }
}