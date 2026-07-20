using Cars24API.Models;
using MongoDB.Driver;

namespace Cars24API.Services
{
    public class CarService
    {
        private readonly IMongoCollection<Car> _cars;
        private readonly PricingService _pricingService;

        public CarService(IConfiguration config, PricingService pricingService)
        {
            _pricingService = pricingService;

            var client = new MongoClient(config.GetConnectionString("Cars24DB"));
            var database = client.GetDatabase(config["MongoDB:DatabaseName"]);
            _cars = database.GetCollection<Car>("Cars");
        }

        private static double CalculateDistance(
    double lat1,
    double lon1,
    double lat2,
    double lon2)
        {
            const double earthRadius = 6371; // km

            double dLat = DegreesToRadians(lat2 - lat1);
            double dLon = DegreesToRadians(lon2 - lon1);

            double a =
                Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(DegreesToRadians(lat1)) *
                Math.Cos(DegreesToRadians(lat2)) *
                Math.Sin(dLon / 2) *
                Math.Sin(dLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return earthRadius * c;
        }

        private static double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }

        public async Task<List<Car>> GetAllAsync() =>
            await _cars.Find(_ => true).ToListAsync();

        public async Task<Car?> GetByIdAsync(string id, string userCity = "")
        {
            var car = await _cars.Find(c => c.Id == id).FirstOrDefaultAsync();

            if (car == null)
                return null;

            var pricing = _pricingService.Calculate(car, userCity);

            car.RecommendedPrice = pricing.RecommendedPrice;
            car.PricingReason = pricing.Reason;

            return car;
        }

        public async Task<List<Car>> SearchAsync(CarSearchRequest request)
        {
            try
            {
                var cars = await GetAllAsync();
                if (request.UserLatitude.HasValue &&
    request.UserLongitude.HasValue)
                {
                    foreach (var car in cars)
                    {
                        car.Distance = CalculateDistance(
                            request.UserLatitude.Value,
                            request.UserLongitude.Value,
                            car.Latitude,
                            car.Longitude);
                    }
                }

                foreach (var car in cars)
                {
                    var pricing = _pricingService.Calculate(car, request.City ?? "");

                    car.RecommendedPrice = pricing.RecommendedPrice;
                    car.PricingReason = pricing.Reason;
                }

                var query = Normalize(request.Query);
                var terms = query.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                var results = cars
    .Where(car => MatchesFilters(car, request))
    .Select(car => new
    {
        Car = car,
        Score = CalculateScore(car, terms, request)
    })
    .Where(x => terms.Length == 0 || x.Score > 0);

                if (request.UserLatitude.HasValue &&
                    request.UserLongitude.HasValue)
                {
                    return results
                        .OrderBy(x => x.Car.Distance ?? double.MaxValue)
                        .ThenByDescending(x => x.Score)
                        .ThenByDescending(x => x.Car.Popularity)
                        .Select(x => x.Car)
                        .ToList();
                }

                return results
                    .OrderByDescending(x => x.Score)
                    .ThenByDescending(x => x.Car.CreatedAt)
                    .ThenByDescending(x => x.Car.Popularity)
                    .Select(x => x.Car)
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public async Task<List<CarSuggestion>> GetSuggestionsAsync(string query, int limit = 6)
        {
            var normalized = Normalize(query);

            if (string.IsNullOrWhiteSpace(normalized))
                return new List<CarSuggestion>();

            var cars = await GetAllAsync();

            var terms = normalized.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            return cars
                .Select(car => new
                {
                    Car = car,
                    Score = TextScore(
                        Normalize(car.Title),
                        terms
                    )
                })
                .Where(x => x.Score > 0)
                .OrderByDescending(x => x.Score)
                .ThenBy(x => x.Car.Title)
                .Take(limit)
                .Select(x => new CarSuggestion
                {
                    Id = x.Car.Id,
                    Title = x.Car.Title,
                    Year = x.Car.Specs.Year,
                    Fuel = x.Car.Specs.Fuel,
                    Location = x.Car.Location
                })
                .ToList();
        }

        private static bool MatchesFilters(Car car, CarSearchRequest request)
        {
            var price = ParseNumber(car.Price ?? "0");
            var mileage = ParseNumber(car.Specs?.Km ?? "0");

            return
                (request.Brands.Count == 0 ||
                 request.Brands.Any(b =>
                    (car.Title ?? "")
                    .Contains(b, StringComparison.OrdinalIgnoreCase)))

                &&

                (string.IsNullOrWhiteSpace(request.Fuel) ||
                 string.Equals(
                     car.Specs?.Fuel,
                     request.Fuel,
                     StringComparison.OrdinalIgnoreCase))

                &&

                (string.IsNullOrWhiteSpace(request.Transmission) ||
                 string.Equals(
                     car.Specs?.Transmission,
                     request.Transmission,
                     StringComparison.OrdinalIgnoreCase))

                &&

                (!request.MinPrice.HasValue ||
                 price >= request.MinPrice.Value)

                &&

                (!request.MaxPrice.HasValue ||
                 price <= request.MaxPrice.Value)

                &&

                (!request.MinMileage.HasValue ||
                 mileage >= request.MinMileage.Value)

                &&

                (!request.MaxMileage.HasValue ||
                 mileage <= request.MaxMileage.Value)

                &&

                (!request.MinYear.HasValue ||
                 (car.Specs?.Year ?? 0) >= request.MinYear.Value)

                &&

                (!request.MaxYear.HasValue ||
                 (car.Specs?.Year ?? 0) <= request.MaxYear.Value)

                &&

                (string.IsNullOrWhiteSpace(request.City) ||
                 string.Equals(car.City, request.City, StringComparison.OrdinalIgnoreCase) ||
                 (!string.IsNullOrWhiteSpace(car.Location) &&
                  car.Location.Contains(request.City, StringComparison.OrdinalIgnoreCase)));
        }

        private static double CalculateScore(Car car, string[] terms, CarSearchRequest request)
        {
            var searchableText = Normalize(
                $"{car.Title ?? ""} {car.City ?? ""} {car.Location ?? ""} {car.Specs?.Fuel ?? ""} {car.Specs?.Transmission ?? ""}"
            );

            var keywordScore =
                terms.Length == 0
                    ? 0
                    : TextScore(searchableText, terms) * 70;

            var filterScore =
                new[]
                {
                    request.Brands.Count > 0,
                    !string.IsNullOrWhiteSpace(request.City),
                    !string.IsNullOrWhiteSpace(request.Fuel),
                    !string.IsNullOrWhiteSpace(request.Transmission),
                    request.MinPrice.HasValue || request.MaxPrice.HasValue,
                    request.MinMileage.HasValue || request.MaxMileage.HasValue,
                    request.MinYear.HasValue || request.MaxYear.HasValue
                }.Count(x => x) * 4;

            var popularityScore =
                Math.Min(car.Popularity, 1000) / 1000d * 15;

            var created =
                car.CreatedAt == default
                    ? DateTime.UtcNow
                    : car.CreatedAt;

            var age =
                Math.Max(0,
                    (DateTime.UtcNow - created).TotalDays);

            var recencyScore =
                Math.Max(0, 10 - age / 18);

            return keywordScore +
                   filterScore +
                   popularityScore +
                   recencyScore;
        }

        private static double TextScore(string text, string[] terms)
        {
            if (terms.Length == 0)
                return 0;

            var words =
                text.Split(' ',
                    StringSplitOptions.RemoveEmptyEntries);

            double score = 0;

            foreach (var term in terms)
            {
                if (text.Contains(term))
                    score += 1;
                else if (words.Any(word =>
                    LevenshteinDistance(word, term)
                    <= Math.Max(1, term.Length / 3)))
                    score += 0.65;
            }

            return score / terms.Length;
        }

        private static int LevenshteinDistance(string source, string target)
        {
            var d = new int[target.Length + 1];

            for (int j = 0; j <= target.Length; j++)
                d[j] = j;

            for (int i = 1; i <= source.Length; i++)
            {
                int prev = d[0]++;

                for (int j = 1; j <= target.Length; j++)
                {
                    int current = d[j];

                    d[j] = Math.Min(
                        Math.Min(d[j] + 1, d[j - 1] + 1),
                        prev + (source[i - 1] == target[j - 1] ? 0 : 1));

                    prev = current;
                }
            }

            return d[target.Length];
        }

        private static string Normalize(string? value)
        {
            return value?.Trim().ToLowerInvariant() ?? "";
        }

        private static decimal ParseNumber(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return 0;

            var number =
                new string(value
                    .Where(c => char.IsDigit(c) || c == '.')
                    .ToArray());

            if (!decimal.TryParse(number, out var result))
                return 0;

            return value.Contains("lakh",
                StringComparison.OrdinalIgnoreCase)
                ? result * 100000
                : result;
        }

        public async Task CreateAsync(Car car)
        {
            await _cars.InsertOneAsync(car);
        }

    }
}
