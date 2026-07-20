namespace Cars24API.Models
{
    public class CarSuggestion
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public int Year { get; set; }
        public string Fuel { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
    }
}