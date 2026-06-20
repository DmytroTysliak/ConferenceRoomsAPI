namespace ConferenceRoomsAPI.Entities
{
    public class Room
    {
        public Guid Id {  get; set; }

        public string Name { get; set; }

        public int Capacity { get; set; }

        public decimal HourlyPrice { get; set; }

        public ICollection<Service> Services { get; set; } = new List<Service>();

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
