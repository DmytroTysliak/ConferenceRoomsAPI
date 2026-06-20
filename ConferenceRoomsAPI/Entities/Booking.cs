namespace ConferenceRoomsAPI.Entities
{
    public class Booking
    {
        public Guid Id { get; set; }

        public int RoomId { get; set; }

        public Room Room { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public decimal TotalPrice { get; set; }

        public ICollection<Service> Services { get; set; } = new List<Service>();
    }
}
