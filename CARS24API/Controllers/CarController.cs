using Microsoft.AspNetCore.Mvc;
using Cars24API.Models;
using Cars24API.Services;

namespace Cars24API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CarController : ControllerBase
    {
        private readonly CarService _carservice;
        private readonly IWebHostEnvironment _environment;
        private readonly UserService _userService;
        private readonly NotificationService _notificationService;

        public CarController(
            CarService carService,
            IWebHostEnvironment environment,
            UserService userService,
            NotificationService notificationService)
        {
            _carservice = carService;
            _environment = environment;
            _userService = userService;
            _notificationService = notificationService;
        }

        // Get Car by Id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(
    string id,
    [FromQuery] string? city)
        {
            var car = await _carservice.GetByIdAsync(id, city ?? "");

            if (car == null)
            {
                return NotFound(new
                {
                    message = "Car not found",
                    id = id
                });
            }

            return Ok(car);
        }

        // Get All Cars (Summary)
        [HttpGet("summaries")]
        public async Task<IActionResult> GetCarSummaries()
        {
            var cars = await _carservice.GetAllAsync();

            return Ok(cars.Select(ToSummary));
        }

        // Search Cars
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] CarSearchRequest request)
        {
            var cars = await _carservice.SearchAsync(request);

            return Ok(cars.Select(ToSummary));
        }

        // Suggestions
        [HttpGet("suggestions")]
        public async Task<IActionResult> Suggestions(
            [FromQuery] string query,
            [FromQuery] int limit = 6)
        {
            var suggestions = await _carservice.GetSuggestionsAsync(query, limit);

            return Ok(suggestions);
        }

        // Create Car
        [HttpPost]
        public async Task<IActionResult> Create([FromQuery] string userId, [FromBody] Car car)
        {
            if (car == null)
            {
                return BadRequest("Car data is required.");
            }

            var user = await _userService.GetByIdAsync(userId);
            if (user == null)
                return NotFound(new { message = "User not found." });

            await _carservice.CreateAsync(car);

            await _notificationService.SendNotificationAsync(
                userId,
                "Car listed successfully",
                $"Your {car.Title} has been listed successfully.");

            return CreatedAtAction(nameof(GetById), new { id = car.Id }, car);
        }

        [HttpPost("upload")]
        [RequestSizeLimit(50_000_000)]
        public async Task<IActionResult> UploadImages([FromForm] List<IFormFile> images)
        {
            const long maxFileSize = 5_000_000;
            var extensions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["image/jpeg"] = ".jpg",
                ["image/png"] = ".png"
            };

            if (images.Count is < 1 or > 10)
                return BadRequest("Upload between 1 and 10 images.");

            if (images.Any(image => image.Length is 0 or > maxFileSize ||
                                    !extensions.ContainsKey(image.ContentType)))
                return BadRequest("Each image must be a JPEG or PNG file no larger than 5 MB.");

            var uploadDirectory = Path.Combine(_environment.ContentRootPath, "wwwroot", "uploads");
            Directory.CreateDirectory(uploadDirectory);

            var urls = new List<string>();
            foreach (var image in images)
            {
                var fileName = $"{Guid.NewGuid():N}{extensions[image.ContentType]}";
                var path = Path.Combine(uploadDirectory, fileName);

                await using var stream = System.IO.File.Create(path);
                await image.CopyToAsync(stream);
                urls.Add($"{Request.Scheme}://{Request.Host}/uploads/{fileName}");
            }

            return Ok(new { urls });
        }

        // Convert to Summary
        private static object ToSummary(Car car) => new
        {
            id = car.Id,
            title = car.Title,
            km = car.Specs.Km,
            year = car.Specs.Year,
            fuel = car.Specs.Fuel,
            transmission = car.Specs.Transmission,
            owner = car.Specs.Owner,
            emi = car.Emi,

            // Original price
            price = car.Price,

            // Dynamic pricing
            recommendedPrice = car.RecommendedPrice,
            pricingReason = car.PricingReason,

            location = car.Location,
            image = car.Images.FirstOrDefault() ?? string.Empty,
            distance = car.Distance
        };
    }
}
