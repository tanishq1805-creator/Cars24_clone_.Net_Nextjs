using Cars24API.Models;
using System.Globalization;

namespace Cars24API.Services
{
    public class PricingResult
    {
        public decimal RecommendedPrice { get; set; }
        public decimal Multiplier { get; set; }
        public string Reason { get; set; } = "";
    }

    public class PricingService
    {
        public PricingResult Calculate(Car car, string userCity)
        {
            decimal multiplier = 1.0m;
            string reason = "Standard market price";

            decimal basePrice = ParsePrice(car.Price);

            string season = GetCurrentSeason();

            // Monsoon SUVs
            if (season == "Monsoon" &&
                (car.Category == "SUV" || car.Category == "OffRoad"))
            {
                multiplier = 1.08m;
                reason = "High demand during monsoon";
            }

            // Metro hatchbacks
            if (IsMetro(userCity) && car.Category == "Hatchback")
            {
                multiplier = 0.95m;
                reason = "Lower demand in metro cities";
            }

            // Hilly region SUVs
            if (IsHillyRegion(userCity) && car.Category == "SUV")
            {
                multiplier = 1.10m;
                reason = "Higher demand in hilly regions";
            }

            return new PricingResult
            {
                RecommendedPrice = basePrice * multiplier,
                Multiplier = multiplier,
                Reason = reason
            };
        }

        private decimal ParsePrice(string price)
        {
            price = price.Replace("₹", "")
                         .Replace(",", "")
                         .Trim();

            decimal.TryParse(price,
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out decimal result);

            return result;
        }

        private string GetCurrentSeason()
        {
            int month = DateTime.Now.Month;

            if (month >= 6 && month <= 9)
                return "Monsoon";

            if (month >= 3 && month <= 5)
                return "Summer";

            return "Winter";
        }

        private bool IsMetro(string city)
        {
            string[] metros =
            {
                "Mumbai",
                "Delhi",
                "Pune",
                "Bangalore",
                "Hyderabad",
                "Chennai",
                "Kolkata"
            };

            return metros.Contains(city, StringComparer.OrdinalIgnoreCase);
        }

        private bool IsHillyRegion(string city)
        {
            string[] hills =
            {
                "Shimla",
                "Manali",
                "Darjeeling",
                "Mussoorie",
                "Nainital"
            };

            return hills.Contains(city, StringComparer.OrdinalIgnoreCase);
        }
    }
}