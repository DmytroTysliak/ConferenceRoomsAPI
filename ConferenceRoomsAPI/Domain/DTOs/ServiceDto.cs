namespace ConferenceRoomsAPI.Domain.DTOs
{
    public class ServiceDto
    {
        public record CreateServiceRequest(string Name, decimal Price);

        public record UpdateServiceRequest(string? Name, decimal? Price);

        public record ServiceResponse(Guid Id, string Name, decimal Price);
    }
}
