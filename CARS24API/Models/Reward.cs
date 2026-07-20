using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Cars24API.Models
{
    public class Reward
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string Title { get; set; } = "";

        public string Description { get; set; } = "";

        public int PointsRequired { get; set; }

        public string Category { get; set; } = "";

        public bool IsActive { get; set; } = true;

        public string ImageUrl { get; set; } = "";
    }
}