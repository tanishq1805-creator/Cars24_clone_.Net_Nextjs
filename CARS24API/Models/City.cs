using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class City
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string Name { get; set; } = "";

    public string State { get; set; } = "";

    public double Latitude { get; set; }

    public double Longitude { get; set; }
}