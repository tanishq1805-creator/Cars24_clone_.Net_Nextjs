using System.Text.Json;
using Cars24API.Models;
using MongoDB.Driver;

namespace Cars24API.Data
{
    public class BrandMaintenanceSeeder
    {
        private readonly IMongoCollection<BrandMaintenanceProfile> _collection;
        private readonly IWebHostEnvironment _environment;

        public BrandMaintenanceSeeder(
            IConfiguration configuration,
            IWebHostEnvironment environment)
        {
            var client = new MongoClient(
                configuration.GetConnectionString("Cars24DB"));

            var database = client.GetDatabase("Cars24DB");

            _collection = database.GetCollection<BrandMaintenanceProfile>(
                "BrandMaintenanceProfiles");

            _environment = environment;
        }

        public async Task SeedAsync()
        {
            var path = Path.Combine(
                _environment.ContentRootPath,
                "Data",
                "brandMaintenance.json");

            if (!File.Exists(path))
                return;

            var json = await File.ReadAllTextAsync(path);

            var profiles = JsonSerializer.Deserialize<List<BrandMaintenanceProfile>>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            if (profiles == null || profiles.Count == 0)
                return;

            foreach (var profile in profiles)
            {
                var exists = await _collection
                    .Find(item => item.Brand == profile.Brand)
                    .AnyAsync();

                if (!exists)
                {
                    await _collection.InsertOneAsync(profile);
                }
            }
        }
    }
}
