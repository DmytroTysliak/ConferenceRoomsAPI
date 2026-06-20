namespace ConferenceRoomsAPI.DTOs
{
    public class CreateRoomDto
    {
        public string Name { get; set; }

        public int Capacity { get; set; }

        public decimal Price { get; set; }

        public List<int> ServiceIds { get; set; }
    }
}
