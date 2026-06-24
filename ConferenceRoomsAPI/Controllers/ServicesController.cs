using ConferenceRoomsAPI.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using static ConferenceRoomsAPI.Domain.DTOs.ServiceDto;

namespace ConferenceRoomsAPI.Controllers
{
    // Manages additional services available in rooms like projector, Wi-Fi and sound)
    [ApiController]
    [Route("api/[controller]")]
    public class ServicesController : ControllerBase
    {
        private readonly ServiceAppService _appService;

        public ServicesController(ServiceAppService appService)
        {
            _appService = appService;
        }

        // Returns all available services
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ServiceResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var services = await _appService.GetAllAsync();
            return Ok(services);
        }

        // Returns a single service by ID
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ServiceResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var service = await _appService.GetByIdAsync(id);
            return service is null ? NotFound() : Ok(service);
        }

        // Creates a new service    
        [HttpPost]
        [ProducesResponseType(typeof(ServiceResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateServiceRequest request)
        {
            var service = await _appService.CreateServiceAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = service.Id }, service);
        }

        // Updates a service. Only provided fields are changed
        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateServiceRequest request)
        {
            await _appService.UpdateServiceAsync(id, request);
            return NoContent();
        }

        // Soft-deletes a service
        [HttpDelete("{id;guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _appService.DeleteServiceAsync(id);
            return NoContent();
        }

    }
}
