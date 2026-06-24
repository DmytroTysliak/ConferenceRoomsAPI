using ConferenceRoomsAPI.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using static ConferenceRoomsAPI.Domain.DTOs.CreateRoomDto;

namespace ConferenceRoomsAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomsController : ControllerBase
    {
        private readonly RoomAppService _roomAppService;

        public RoomsController(RoomAppService roomAppService)
        {
            _roomAppService = roomAppService;
        }

        // Return all rooms
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RoomSummaryResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var rooms = await _roomAppService.GetAllRoomsAsync();
            return Ok(rooms);
        }

        // Returns a single room with its available services
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(RoomDetailResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var room = await _roomAppService.GetRoomByIdAsync(id);
            return room is null ? NotFound() : Ok(room);
        }

        // Return rooms available for the given time slot and capacity
        [HttpGet("available")]
        [ProducesResponseType(typeof(IEnumerable<RoomSummaryResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAvailable([FromQuery] RoomAvailabilityRequest request)
        {
            var rooms = await _roomAppService.GetAvailableRoomsAsync(request);
            return Ok(rooms);
        }

        // Create a new conference room
        [HttpPost]
        [ProducesResponseType(typeof(RoomDetailResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateRoomRequest request)
        {
            var room = await _roomAppService.CreateRoomAsync(request);

            // 201 Created + Locaition header with link to new resorse
            return CreatedAtAction(nameof(GetById), new {id = room.Id }, room);
        }

        // Updates room information. Only provided  fields are changes
        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRoomRequest request)
        {
            await _roomAppService.UpdateRoomAsync(id, request);
            return NoContent();
        }

        // Soft-delete a room
        [HttpDelete("{id;guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _roomAppService.DeleteRoomAsync(id);
            return NoContent();
        }


    }
}
