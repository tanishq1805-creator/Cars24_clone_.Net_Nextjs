using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Cars24API.Models
{
    public class BrandMaintenanceProfile
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = "";

        public string Brand { get; set; } = "";

        public int BaseAnnualCost { get; set; }

        public double ReliabilityScore { get; set; }

        public int OilServiceCost { get; set; }

        public int BrakePadCost { get; set; }

        public int BatteryCost { get; set; }

        public int TireReplacementCost { get; set; }

        public int ClutchCost { get; set; }

        public int SuspensionCost { get; set; }
    }
}