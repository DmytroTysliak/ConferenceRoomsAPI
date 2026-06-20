namespace ConferenceRoomsAPI.Domain.Entities
{
    public class Booking
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid RoomId { get; set; }

        public Room Room { get; set; } = null!;

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public decimal TotalPrice { get; set; }

        public BookingStatus Status { get; set; } = BookingStatus.Confirmed;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<BookingService> BookingServices { get; set; } = new List<BookingService>();
    }
}
