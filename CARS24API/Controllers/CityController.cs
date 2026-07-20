using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class CityController : ControllerBase
{
    private readonly CityService _service;

    public CityController(CityService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        return Ok(await _service.GetAllAsync());
    }
}