using Cars24API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cars24API.Controllers
{
    [ApiController]
    [Route("api/vehicle-care")]
    public class VehicleCareController : ControllerBase
    {
        private readonly MaintenanceService _maintenanceService;
        private readonly CarService _carService;

        public VehicleCareController(
            MaintenanceService maintenanceService,
            CarService carService)
        {
            _maintenanceService = maintenanceService;
            _carService = carService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetVehicleCare(string id)
        {
            var car = await _carService.GetByIdAsync(id);

            if (car == null)
                return NotFound(new
                {
                    success = false,
                    message = "Car not found."
                });

            var prediction = _maintenanceService.PredictMaintenance(car);

            return Ok(new
            {
                success = true,
                data = prediction
            });
        }
    }
}