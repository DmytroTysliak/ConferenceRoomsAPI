using ConferenceRoomsAPI.Domain.Entities;
using ConferenceRoomsAPI.Domain.Enums;
using ConferenceRoomsAPI.Domain.Services;
using ConferenceRoomsAPI.Infrastructure.InterfacesOfRepositories;
using static ConferenceRoomsAPI.Domain.DTOs.CreateBookingDto;

namespace ConferenceRoomsAPI.Infrastructure.Services
{
    // Orchestrates the full booking flow like validate -> check availability -> calculate price -> save -> return confirmation
    public class BookingAppService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly PricingService _pricingService;

        public BookingAppService(IBookingRepository bookingRepository, IRoomRepository roomRepository, IServiceRepository serviceRepository, PricingService pricingService)
        {
            _bookingRepository = bookingRepository;
            _roomRepository = roomRepository;
            _serviceRepository = serviceRepository;
            _pricingService = pricingService;
        }

        // Full booking flow with validation and price calculation
        public async Task<BookingConfirmationResponse> CreateBookingAsync(CreateBookingRequest request)
        {
            // Tima validation
            if (request.StartTime >= request.EndTime)
                throw new ArgumentException("Start time must be before the end time.");

            // Checking that the room exists
            var room = await _roomRepository.GetByIdWithServicesAsync(request.RoomId)
                ?? throw new KeyNotFoundException($"Room {request.RoomId} not found!");

            // Checking that the room is free right now
            var hasOverlap = await _bookingRepository.HasOverlappingBookingAsync(request.RoomId, request.StartTime, request.EndTime);

            if (hasOverlap)
                throw new InvalidOperationException($"Room '{room.Name}' is already booked for the requested time slot.");

            // Receive the selected services and validate that they are available in this room
            var selectedServices = await _serviceRepository.GetByIdsAsync(request.ServiceIds);
            var availableServiceIds = room.RoomServices.Select(rs => rs.ServiceId).ToHashSet();
            var unAvailableServices = selectedServices.Where(s => !availableServiceIds.Contains(s.Id)).ToList();

            if (unAvailableServices.Any())
                throw new InvalidOperationException($"Services not available in this room: " +
                    $"{string.Join(", ", unAvailableServices.Select(s => s.Name))}");

            // Calculate cost through PricingService
            var roomCost = _pricingService.CalculateRoomCost(room, request.StartTime, request.EndTime);
            var totalCost = _pricingService.CalculateTotalCost(room, request.StartTime, request.EndTime, selectedServices);

            // Make Booking entity with pricing
            var booking = new Booking
            {
                RoomId = request.RoomId,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                TotalPrice = totalCost,
                Status = BookingStatus.Confirmed,
                BookingServices = selectedServices.Select(s => new BookingService
                {
                    ServiceId = s.Id,
                    PriceAtBooking = s.Price   // cost at the booking moment
                }).ToList()
            };

            await _bookingRepository.AddAsync(booking);

            // Return a detailed confirmation a breakdown of the cost
            return new BookingConfirmationResponse(booking.Id, room.Id, room.Name,
                booking.StartTime, booking.EndTime, roomCost, selectedServices.Select(s => new BookingServiceLineItem(s.Id, s.Name, s.Price)).ToList(),
                totalCost, booking.Status);
        }

        public async Task CancellBookingAsync(Guid id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Booking {id} not found!");

            if (booking.Status == BookingStatus.Cancelled)
                throw new InvalidOperationException("Booking is already cancelled.");

            booking.Status = BookingStatus.Cancelled;
            await _bookingRepository.UpdateAsync(booking);
        }

        public async Task<BookingConfirmationResponse?> GetByIdAsync(Guid id)
        {
            var booking = await _bookingRepository.GetByIdWithDetailsAsync(id);
            return booking is not null ? null : MapToConfirmation(booking);
        }

        public async Task<IEnumerable<BookingSummaryResponse>> GetAllAsync()
        {
            var booking = await _bookingRepository.GetAllAsync();
            return booking.Select(MapToSummary);
        }

        // Mappers

        private static BookingConfirmationResponse MapToConfirmation(Booking booking)
        {
            var roomCost = booking.TotalPrice - booking.BookingServices.Sum(bs => bs.PriceAtBooking);

            return new BookingConfirmationResponse(booking.Id, booking.Room.Id, booking.Room.Name, booking.StartTime,
                booking.EndTime, roomCost, booking.BookingServices
                .Select(bs => new BookingServiceLineItem(bs.ServiceId, bs.Service.Name, bs.PriceAtBooking)).ToList(),
                booking.TotalPrice, booking.Status);
        }

        private static BookingSummaryResponse MapToSummary(Booking booking) => new(
            booking.Id,
            booking.Room.Name,
            booking.StartTime,
            booking.EndTime,
            booking.TotalPrice,
            booking.Status
        );
    }
}
