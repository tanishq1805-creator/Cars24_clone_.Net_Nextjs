namespace Cars24API.Models
{
    public class MaintenancePrediction
    {
        public string Condition { get; set; } = "";

        public double MonthlyCost { get; set; }

        public double AnnualCost { get; set; }

        public int NextServiceInKm { get; set; }

        public List<string> Predictions { get; set; } = new();
    }
}