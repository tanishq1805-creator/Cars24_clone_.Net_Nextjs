using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
namespace Cars24API.Models;

public class Specs
{
    public string Brand { get; set; } = "";

    public string Model { get; set; } = "";
    public int Year { get; set; }
    public string Km { get; set; } = string.Empty;
    public string Fuel { get; set; } = string.Empty;
    public string Transmission { get; set; } = string.Empty;
    public string Owner { get; set; } = string.Empty;
    public string Insurance { get; set; } = string.Empty;
}


public class Car
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    public List<string> Images { get; set; } = new();

    public string Title { get; set; } = string.Empty;

    public string Price { get; set; } = string.Empty;

    public string Emi { get; set; } = string.Empty;

    public string Location { get; set; } = string.Empty;

    // NEW
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;

    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public Specs Specs { get; set; } = new();

    public List<string> Features { get; set; } = new();

    public List<string> Highlights { get; set; } = new();

    public int Popularity { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonIgnore]
    public double? Distance { get; set; }

    public string Category { get; set; } = "";

    [BsonIgnore]
    public decimal RecommendedPrice { get; set; }

    [BsonIgnore]
    public string PricingReason { get; set; } = "";
}
