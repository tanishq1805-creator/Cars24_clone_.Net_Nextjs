using Cars24API.Models;

namespace Cars24API.Services
{
    public class MaintenanceService
    {
        public MaintenancePrediction PredictMaintenance(Car car)
        {
            int currentYear = DateTime.Now.Year;

            int age = DateTime.Now.Year - car.Specs.Year;
            int km = 0;

            int.TryParse(
                car.Specs.Km.Replace(",", "").Replace("km", "").Trim(),
                out km
            );

            double baseAnnualCost = 18000;

            double ageMultiplier = 1.0;

            if (age <= 3)
                ageMultiplier = 1.0;
            else if (age <= 6)
                ageMultiplier = 1.25;
            else if (age <= 10)
                ageMultiplier = 1.50;
            else
                ageMultiplier = 1.80;

            double kmMultiplier = 1.0;

            if (km <= 30000)
                kmMultiplier = 1.0;
            else if (km <= 60000)
                kmMultiplier = 1.15;
            else if (km <= 90000)
                kmMultiplier = 1.35;
            else if (km <= 120000)
                kmMultiplier = 1.60;
            else
                kmMultiplier = 1.90;

            double fuelMultiplier = 1.0;

            switch (car.Specs.Fuel.ToLower())
            {
                case "diesel":
                    fuelMultiplier = 1.10;
                    break;

                case "petrol":
                    fuelMultiplier = 1.0;
                    break;

                case "hybrid":
                    fuelMultiplier = 1.05;
                    break;

                case "electric":
                    fuelMultiplier = 0.80;
                    break;
            }

            double transmissionMultiplier = 1.0;

            if (car.Specs.Transmission.ToLower().Contains("automatic"))
                transmissionMultiplier = 1.10;

            double annualCost =
                baseAnnualCost
    * ageMultiplier
    * kmMultiplier
    * fuelMultiplier
    * transmissionMultiplier;

            double monthlyCost = annualCost / 12;

            string condition;

            if (annualCost < 22000)
                condition = "Low Maintenance";
            else if (annualCost < 32000)
                condition = "Medium Maintenance";
            else
                condition = "High Maintenance Expected";

            List<string> predictions = new();

            if (km >= 40000)
                predictions.Add("Brake pads likely need inspection.");

            if (km >= 60000)
                predictions.Add("Battery replacement may be due soon.");

            if (km >= 80000)
                predictions.Add("Tire replacement expected soon.");

            if (km >= 100000)
                predictions.Add("Suspension components should be inspected.");

            if (age >= 7)
                predictions.Add("Rubber hoses and belts should be checked.");

            int nextService = 10000 - (km % 10000);

            if (nextService == 10000)
                nextService = 0;

            return new MaintenancePrediction
            {
                Condition = condition,
                MonthlyCost = Math.Round(monthlyCost),
                AnnualCost = Math.Round(annualCost),
                NextServiceInKm = nextService,
                Predictions = predictions
            };
        }
    }

}
