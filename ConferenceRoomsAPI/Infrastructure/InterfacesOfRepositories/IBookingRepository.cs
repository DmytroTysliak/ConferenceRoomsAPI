using ConferenceRoomsAPI.Domain.Entities;

namespace ConferenceRoomsAPI.Infrastructure.InterfacesOfRepositories
{
    public interface IBookingRepository
    {
        Task<Booking?> GetByIdAsync(Guid Id);

        // Includes Room + BookingServices -> Service
        Task<Booking?> GetByIdWithDetailsAsync(Guid Id);
        Task<IEnumerable<Booking>> GetAllAsync();
        Task<IEnumerable<Booking>> GetByRoomIdAsync(Guid RoomId);
        Task<IEnumerable<Booking>> GetByDateRangeAsync(DateTime From, DateTime To);
        Task<bool> HasOverlappingBookingAsync(Guid RoomId, DateTime Start, DateTime End);
        Task AddAsync(Booking Booking);
        Task UpdateAsync(Booking Booking);
    }
}
