using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Cars24API.Models
{
    public class CarModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        public string Brand { get; set; } = "";

        public string Model { get; set; } = "";

        public string Segment { get; set; } = "";

        public string FuelType { get; set; } = "";

        public string BodyType { get; set; } = "";

        public bool IsElectric { get; set; }
    }
}