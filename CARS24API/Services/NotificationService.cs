using Cars24API.Models;
using MongoDB.Driver;
using FirebaseAdmin.Messaging;

namespace Cars24API.Services
{
    public class NotificationService
    {
        private readonly IMongoCollection<NotificationToken> _tokens;

        public NotificationService(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("Cars24DB"));
            var database = client.GetDatabase(config["MongoDB:DatabaseName"]);

            _tokens = database.GetCollection<NotificationToken>("NotificationTokens");
        }

        public async Task SaveTokenAsync(RegisterNotificationTokenRequest request)
        {
            var existing = await _tokens.Find(x =>
                    x.UserId == request.UserId &&
                    x.Token == request.Token)
                .FirstOrDefaultAsync();

            if (existing != null)
                return;

            await _tokens.InsertOneAsync(new NotificationToken
            {
                UserId = request.UserId,
                Token = request.Token,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
        }

        public async Task<List<NotificationToken>> GetUserTokens(string userId)
        {
            return await _tokens
                .Find(x => x.UserId == userId)
                .ToListAsync();
        }

        public async Task SendNotificationAsync(
    string userId,
    string title,
    string body)
        {
            var tokens = await _tokens
                .Find(x => x.UserId == userId)
                .ToListAsync();

            foreach (var token in tokens)
            {
                try
                {
                    var message = new Message
                    {
                        Token = token.Token,

                        Notification = new Notification
                        {
                            Title = title,
                            Body = body
                        },

                        Data = new Dictionary<string, string>
                {
                    { "click_action", "OPEN_APP" }
                }
                    };

                    await FirebaseMessaging.DefaultInstance.SendAsync(message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
