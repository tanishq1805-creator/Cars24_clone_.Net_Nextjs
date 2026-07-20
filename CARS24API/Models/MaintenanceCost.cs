using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Cars24API.Models
{
    public class MaintenanceCost
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        public string Brand { get; set; } = "";

        public string Model { get; set; } = "";

        // Average yearly maintenance
        public double BaseAnnualMaintenance { get; set; }

        public double OilServiceCost { get; set; }

        public double BrakePadCost { get; set; }

        public double BatteryCost { get; set; }

        public double TireReplacementCost { get; set; }

        public double ClutchCost { get; set; }

        public double SuspensionCost { get; set; }
    }
}