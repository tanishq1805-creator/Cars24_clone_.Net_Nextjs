using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Cars24API.Models
{
    public class RewardRedemption
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; } = "";

        [BsonRepresentation(BsonType.ObjectId)]
        public string RewardId { get; set; } = "";

        public int PointsSpent { get; set; }

        public DateTime RedeemedAt { get; set; } = DateTime.UtcNow;

        public string Status { get; set; } = "Redeemed";
    }

}

namespace Cars24API.Models
{
    public class RewardRedeemRequest
    {
        public string UserId { get; set; } = "";

        public string RewardId { get; set; } = "";
    }
}
