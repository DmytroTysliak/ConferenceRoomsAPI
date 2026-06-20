namespace ConferenceRoomsAPI.Domain.Entities
{
    public class Service
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Name { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public bool IsDeleted { get; set; }

        public ICollection<RoomService> RoomServices { get; set; } = new List<RoomService>();
        public ICollection<BookingService> BookingServices { get; set; } = new List<BookingService>();
    }
}
