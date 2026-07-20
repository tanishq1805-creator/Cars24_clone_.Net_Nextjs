using MongoDB.Driver;
using Cars24API.Data;
using Cars24API.Services;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

var builder = WebApplication.CreateBuilder(args);

// -------------------------
// Add Services
// -------------------------

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

// MongoDB Connection String
string? connectionString = builder.Configuration.GetConnectionString("Cars24DB");

// Dependency Injection
builder.Services.AddSingleton<UserService>();
builder.Services.AddSingleton<CarService>();
builder.Services.AddSingleton<BookingService>();
builder.Services.AddSingleton<AppointmentService>();
builder.Services.AddSingleton<CityService>();
builder.Services.AddSingleton<PricingService>();
builder.Services.AddSingleton<NotificationService>();
builder.Services.AddSingleton<ReferralService>();
builder.Services.AddSingleton<RewardService>();
builder.Services.AddSingleton<MaintenanceService>();
builder.Services.AddSingleton<DatabaseSeeder>();
builder.Services.AddSingleton<ImportService>();
builder.Services.AddSingleton<BrandMaintenanceSeeder>();

// Firebase credentials are secrets and must be supplied by the deployment
// environment, not committed to the repository. On Render, set
// Firebase__ServiceAccountJson to the complete service-account JSON value.
var firebaseServiceAccountJson = builder.Configuration["Firebase:ServiceAccountJson"];
var firebaseCredentialsPath = builder.Configuration["Firebase:CredentialsPath"]
    ?? "Firebase/cars24-clone-d0698-firebase-adminsdk-fbsvc-5ece05e6b1.json";

if (!string.IsNullOrWhiteSpace(firebaseServiceAccountJson))
{
    FirebaseApp.Create(new AppOptions
    {
        Credential = GoogleCredential.FromJson(firebaseServiceAccountJson)
    });
}
else if (File.Exists(firebaseCredentialsPath))
{
    FirebaseApp.Create(new AppOptions
    {
        Credential = GoogleCredential.FromFile(firebaseCredentialsPath)
    });
}
else
{
    Console.WriteLine("Firebase is not configured; push notifications are disabled.");
}

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// -------------------------
// Build App
// -------------------------

var app = builder.Build();

// Create the application collections on startup so they are visible in MongoDB
// Atlas even before the first document is inserted.
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("The Cars24DB connection string is missing.");
}

var databaseName = builder.Configuration["MongoDB:DatabaseName"];
if (string.IsNullOrWhiteSpace(databaseName))
{
    throw new InvalidOperationException("The MongoDB database name is missing.");
}

var mongoDatabase = new MongoClient(connectionString).GetDatabase(databaseName);
var existingCollections = (await mongoDatabase.ListCollectionNamesAsync()).ToList();
var requiredCollections = new[]
{
    "Users",
    "Cars",
    "Bookings",
    "Appointments",
    "NotificationTokens",
    "Cities",
    "ReferralWallets",
    "WalletTransactions",
    "ReferralRewards",
    "Rewards",
    "RewardRedemptions",
    "BrandMaintenanceProfiles"
};

foreach (var collectionName in requiredCollections.Except(existingCollections))
{
    await mongoDatabase.CreateCollectionAsync(collectionName);
}

// Accounts created before referral codes were generated may have an empty code.
// Assign a unique code to each of those accounts on startup.
var userService = app.Services.GetRequiredService<UserService>();
var referralService = app.Services.GetRequiredService<ReferralService>();
await userService.AssignMissingReferralCodesAsync(referralService);

// -------------------------
// Configure Middleware
// -------------------------

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");
app.UseStaticFiles();

// -------------------------
// Test Routes
// -------------------------

app.MapGet("/", () => "Welcome to Cars24 API");

app.MapGet("/db-check", async () =>
{
    try
    {
        var client = new MongoClient(connectionString);
        await client.ListDatabaseNamesAsync();

        return Results.Ok("MongoDB Connected Successfully");
    }
    catch (Exception ex)
    {
        return Results.Problem($"MongoDB Connection Failed: {ex.Message}");
    }
});

// -------------------------
// Controllers
// -------------------------

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    await services.GetRequiredService<DatabaseSeeder>().SeedAsync();

    await services.GetRequiredService<BrandMaintenanceSeeder>().SeedAsync();
}
app.Run();

