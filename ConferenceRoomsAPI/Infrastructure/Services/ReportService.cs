using ConferenceRoomsAPI.Domain.Enums;
using ConferenceRoomsAPI.Infrastructure.InterfacesOfRepositories;
using static ConferenceRoomsAPI.Domain.DTOs.ReportDto;

namespace ConferenceRoomsAPI.Infrastructure.Services
{
    // Generates business analytics reports from booking data
    public class ReportService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly IServiceRepository _serviceRepository;

        public ReportService(IBookingRepository bookingRepository, IRoomRepository roomRepository, IServiceRepository serviceRepository)
        {
            _bookingRepository = bookingRepository;
            _roomRepository = roomRepository;
            _serviceRepository = serviceRepository;
        }

        // Revenue per room for a given date range. Only counts confirmed bookings
        public async Task<OccupancyReportResponse> GetRevenueReportAsync(DateTime from, DateTime to)
        {
            var bookings = await _bookingRepository.GetByDateRangeAsync(from, to);
            var confirmedBookings = bookings.Where(b => b.Status == BookingStatus.Confirmed).ToList(); 

            var rooms = await _roomRepository.GetAllAsync();

            var roomRevenue = rooms.Select(room =>
            {
                var roomBookings = confirmedBookings.Where(b => b.RoomId == room.Id).ToList();

                return new RoomRevenueItem(room.Id, room.Name, roomBookings.Count, roomBookings.Sum(b => b.TotalPrice));
            }).ToList();


            return new OccupancyReportResponse(
                from,
                to,
                roomRevenue,
                roomRevenue.Sum(r => r.TotalRevenue)
            );
        }

        // How many hours each room was occupied VS available in the date range
        public async Task<RoomOccupacyResponse> GetOccupacyReportAsync(DateTime from, DateTime to)
        {
            var bookings = await _bookingRepository.GetByDateRangeAsync(from, to);
            var confirmedBookings = bookings.Where(b => b.Status == BookingStatus.Confirmed).ToList();

            var rooms = await _roomRepository.GetAllAsync();

            // Available hours = difference between from and to (working hours 06:00–23:00 = 17 hours/day)
            var totalDays = (to - from).TotalDays;
            var availableHoursPerRoom = totalDays * 17;

            var ocupancy = rooms.Select(room =>
            {
                var bookedHours = confirmedBookings.Where(b => b.RoomId == room.Id).Sum(b => (b.EndTime - b.StartTime).TotalHours);

                var occupancyPercent = availableHoursPerRoom > 0 ? Math.Round(bookedHours / availableHoursPerRoom * 100, 1) : 0;

                return new RoomOccupacyItem(
                    room.Id,
                    room.Name,
                    Math.Round(bookedHours, 1),
                    Math.Round(availableHoursPerRoom, 1),
                    occupancyPercent
                );
            }).ToList();

            return new RoomOccupacyResponse(from, to, ocupancy);
        }

        // Which services are ordered most often across all bookings
        public async Task<PopularServicesReportResponse> GetPopularServicesReportAsync()
        {
            var bookings = await _bookingRepository.GetAllAsync();
            var confirmedBookings = bookings.Where(b => b.Status == BookingStatus.Confirmed).ToList();

            var serviceStats = confirmedBookings.SelectMany(b => b.BookingServices)
                .GroupBy(bs => bs.ServiceId)
                .Select(group => new ServicePopularItem(
                    group.Key,
                    group.First().Service.Name,
                    group.Count(),
                    group.Sum(bs => bs.PriceAtBooking)
                ))
                .OrderByDescending(s => s.TimesOrdered)
                .ToList();

            return new PopularServicesReportResponse(serviceStats);
        }
    }
}
