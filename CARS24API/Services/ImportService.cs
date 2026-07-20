using System.Text.Json;
using Cars24API.Models;
using MongoDB.Driver;

namespace Cars24API.Services;

public class ImportService
{
    private readonly IMongoCollection<CarBrand> _brandCollection;
    private readonly IMongoCollection<CarModel> _modelCollection;
    private readonly IWebHostEnvironment _environment;

    public ImportService(
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        var client = new MongoClient(configuration.GetConnectionString("Cars24DB"));
        var database = client.GetDatabase("Cars24DB");

        _brandCollection = database.GetCollection<CarBrand>("CarBrands");
        _modelCollection = database.GetCollection<CarModel>("CarModels");
        _environment = environment;
    }

    public async Task<int> ImportBrandsAsync()
    {
        var path = Path.Combine(_environment.ContentRootPath, "Data", "brands.json");
        var brands = await ReadFileAsync<List<CarBrand>>(path);
        var importedCount = 0;

        foreach (var brand in brands)
        {
            var exists = await _brandCollection
                .Find(item => item.Name == brand.Name)
                .AnyAsync();

            if (!exists)
            {
                await _brandCollection.InsertOneAsync(brand);
                importedCount++;
            }
        }

        return importedCount;
    }

    public async Task<int> ImportModelsAsync()
    {
        var path = Path.Combine(_environment.ContentRootPath, "Data", "Models.json");
        var models = await ReadFileAsync<List<CarModel>>(path);
        var importedCount = 0;

        foreach (var model in models)
        {
            var exists = await _modelCollection
                .Find(item => item.Brand == model.Brand && item.Model == model.Model)
                .AnyAsync();

            if (!exists)
            {
                await _modelCollection.InsertOneAsync(model);
                importedCount++;
            }
        }

        return importedCount;
    }

    private static async Task<T> ReadFileAsync<T>(string path) where T : new()
    {
        var json = await File.ReadAllTextAsync(path);
        return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? new T();
    }
}
