using ConferenceRoomsAPI.Domain.Entities;
using ConferenceRoomsAPI.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using static ConferenceRoomsAPI.Domain.DTOs.ReportDto;

namespace ConferenceRoomsAPI.Controllers
{
    // Business analytics — revenue, occupancy, and popular services reports.
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly ReportService _reportService;

        public ReportsController(ReportService reportService)
        {
            _reportService = reportService;
        }

        // Returns total revenue per room for a given date range
        [HttpGet("revenue")]
        [ProducesResponseType(typeof(OccupancyReportResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetRevenue([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            if (from >= to)
                return BadRequest("'from' must be earlier than 'to'.");

            var report = await _reportService.GetRevenueReportAsync(from, to);
            return Ok(report);
        }

        // Returns occupancy percentage per room for a given date range. Based on 17 available working hours per day (06:00–23:00)
        [HttpGet("occupancy")]
        [ProducesResponseType(typeof(RoomOccupacyResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetOccupancy([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            if (from >= to)
                return BadRequest("'from' must be earlier than 'to'.");

            var report = await _reportService.GetOccupacyReportAsync(from, to);
            return Ok(report);
        }

        // Returns services ranked by how often they were ordered
        [HttpGet("popular-services")]
        [ProducesResponseType(typeof(PopularServicesReportResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPopularServices()
        {
            var report = await _reportService.GetPopularServicesReportAsync();
            return Ok(report);
        }
    }
}
