public class CarSearchRequest
{

    public string? City { get; set; }

    public string? Query { get; set; }

    public List<string> Brands { get; set; } = new();

    public string? Fuel { get; set; }

    public string? Transmission { get; set; }

    public decimal? MinPrice { get; set; }

    public decimal? MaxPrice { get; set; }

    public int? MinMileage { get; set; }

    public int? MaxMileage { get; set; }

    public int? MinYear { get; set; }

    public int? MaxYear { get; set; }

    public double? UserLatitude { get; set; }

    public double? UserLongitude { get; set; }
}