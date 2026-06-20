namespace ConferenceRoomsAPI.Domain.DTOs
{
    public class CreateRoomDto
    {
        public string Name { get; set; }

        public int Capacity { get; set; }

        public decimal HourlyPrice { get; set; }

        public List<int> ServiceIds { get; set; }
    }
}
