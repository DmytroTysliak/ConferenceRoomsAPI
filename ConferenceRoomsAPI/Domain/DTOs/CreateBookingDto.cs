using ConferenceRoomsAPI.Domain.Enums;

namespace ConferenceRoomsAPI.Domain.DTOs
{
    public class CreateBookingDto
    {
        public record CreateBookingRequest(Guid RoomId, DateTime StartTime, DateTime EndTime, List<Guid> ServiceIds);

        public record BookingConfirmationResponse(Guid BookingRoomId, Guid RoomId, string RoomName, 
            DateTime StartTime, DateTime EndTime, decimal RoomCost, List<BookingServiceLineItem> Services, decimal TotalCost, BookingStatus Status);

        public record BookingServiceLineItem(Guid ServiceId, string ServiceName, decimal Price);

        public record BookingSummaryResponse(Guid BookingId, string RoomName, 
            DateTime StartTime, DateTime EndTime, decimal TotalCost, BookingStatus Status);
    }
}
