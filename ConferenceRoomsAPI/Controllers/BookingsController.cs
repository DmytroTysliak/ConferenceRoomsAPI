using ConferenceRoomsAPI.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using static ConferenceRoomsAPI.Domain.DTOs.CreateBookingDto;

namespace ConferenceRoomsAPI.Controllers
{
    // Manages room bookings — create, cancel, and retrieve booking details
    [ApiController]
    [Route("api/[controller]")]
    public class BookingsController : ControllerBase
    {
        private readonly BookingAppService _bookingAppService;

        public BookingsController(BookingAppService bookingAppService)
        {
            _bookingAppService = bookingAppService;
        }

        // Returns all bookings
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<BookingSummaryResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var bookings = await _bookingAppService.GetAllAsync();
            return Ok(bookings);
        }

        // Returns a single booking with full cost breakdown
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(BookingSummaryResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var booking = await _bookingAppService.GetByIdAsync(id);
            return booking is null ? NotFound() : Ok(booking);
        }

        // Create a new booking for a new room. Returns full confirmation with cost breakdown
        [HttpPost]
        [ProducesResponseType(typeof(BookingSummaryResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Create([FromBody] CreateBookingRequest request)
        {
            var booking = await _bookingAppService.CreateBookingAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = booking.BookingRoomId }, booking);
        }

        // Cancelers an existing booking
        [HttpPatch("{id:guid}/cancel")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Cancel(Guid id)
        {
            await _bookingAppService.CancellBookingAsync(id);
            return NoContent();
        }
    }
}
