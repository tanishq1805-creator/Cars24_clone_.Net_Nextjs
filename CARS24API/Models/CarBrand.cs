using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Cars24API.Models
{
    public class CarBrand
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        public string Name { get; set; } = "";

        public string Country { get; set; } = "";

        public string Logo { get; set; } = "";

        public double ReliabilityScore { get; set; }

        public double BaseAnnualMaintenance { get; set; }
    }
}