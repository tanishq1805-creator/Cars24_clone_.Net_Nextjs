using Cars24API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cars24API.Controllers
{
    [ApiController]
    [Route("api/import")]
    public class ImportController : ControllerBase
    {
        private readonly ImportService _importService;

        public ImportController(ImportService importService)
        {
            _importService = importService;
        }

        [HttpPost("brands")]
        public async Task<IActionResult> ImportBrands()
        {
            var count = await _importService.ImportBrandsAsync();

            return Ok(new
            {
                Message = $"{count} brands imported successfully."
            });
        }

        [HttpPost("models")]
        public async Task<IActionResult> ImportModels()
        {
            var count = await _importService.ImportModelsAsync();

            return Ok(new
            {
                Message = $"{count} models imported successfully."
            });
        }
    }
}