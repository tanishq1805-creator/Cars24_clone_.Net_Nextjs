using System.Text.Json;
using Cars24API.Models;
using MongoDB.Driver;

namespace Cars24API.Data
{
    public class DatabaseSeeder
    {
        private readonly IMongoDatabase _database;
        private readonly IWebHostEnvironment _environment;

        public DatabaseSeeder(
            IConfiguration configuration,
            IWebHostEnvironment environment)
        {
            var client = new MongoClient(
                configuration.GetConnectionString("Cars24DB"));

            _database = client.GetDatabase("Cars24DB");
            _environment = environment;
        }

        public async Task SeedAsync()
        {
            var brandCollection =
                _database.GetCollection<CarBrand>("CarBrands");

            var modelCollection =
                _database.GetCollection<CarModel>("CarModels");

            var maintenanceCollection =
                _database.GetCollection<MaintenanceProfile>("MaintenanceProfiles");

            // Prevent duplicate seeding
            if (await brandCollection.CountDocumentsAsync(_ => true) > 0)
                return;

            var dataPath = Path.Combine(_environment.ContentRootPath, "Data");

            var brandsJson =
                await File.ReadAllTextAsync(Path.Combine(dataPath, "brands.json"));

            var modelsJson =
                await File.ReadAllTextAsync(Path.Combine(dataPath, "models.json"));

            var maintenanceJson =
                await File.ReadAllTextAsync(Path.Combine(dataPath, "maintenanceProfiles.json"));

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var brands =
                JsonSerializer.Deserialize<List<CarBrand>>(brandsJson, options)
                ?? new();

            var models =
                JsonSerializer.Deserialize<List<CarModel>>(modelsJson, options)
                ?? new();

            var maintenance =
                JsonSerializer.Deserialize<List<MaintenanceProfile>>(maintenanceJson, options)
                ?? new();

            if (brands.Any())
                await brandCollection.InsertManyAsync(brands);

            if (models.Any())
                await modelCollection.InsertManyAsync(models);

            if (maintenance.Any())
                await maintenanceCollection.InsertManyAsync(maintenance);
        }
    }
}