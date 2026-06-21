using static ConferenceRoomsAPI.Domain.DTOs.ServiceDto;

namespace ConferenceRoomsAPI.Domain.DTOs
{
    public class CreateRoomDto
    {
        public record CreateRoomRequest(string Name, int Capacity, decimal HourlyPrice, List<Guid> ServiceIds);

        public record UpdateRoomRequest(string? Name, int? Capacity, decimal? HourlyPrice, List<Guid>? ServiceIds);

        public record RoomAvailabilityRequest(DateTime StartTime, DateTime EndTime, int MinCapacity);

        public record RoomSummaryResponse(Guid Id, string Name, int Capacity, decimal HourlyPrice);

        public record RoomDetailResponse(Guid Id, string Name, int Capacity, decimal HourlyPrice, 
            List<ServiceResponse> AvailableServices, DateTime CreatedAt, DateTime UpdatedAt);
    }
}
