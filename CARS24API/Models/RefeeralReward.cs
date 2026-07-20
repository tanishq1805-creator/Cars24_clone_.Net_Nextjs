using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Cars24API.Models
{
    public class ReferralReward
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string ReferrerId { get; set; } = "";

        [BsonRepresentation(BsonType.ObjectId)]
        public string ReferredUserId { get; set; } = "";

        public int ReferrerPoints { get; set; }

        public int ReferredUserPoints { get; set; }

        // Purchase, Sale...
        public string Trigger { get; set; } = "";

        public DateTime RewardedAt { get; set; } = DateTime.UtcNow;
        public DateTime RewardDate { get; internal set; }
    }
}