using ConferenceRoomsAPI.Domain.Entities;

namespace ConferenceRoomsAPI.Infrastructure.InterfacesOfRepositories
{
    public interface IRoomRepository
    {
        Task<Room?> GetByIdAsync(Guid Id);
        
        // Includes RoomServices -> Service
        Task<Room?> GetByIdWithServicesAsync(Guid Id);
        Task<IEnumerable<Room>> GetAllAsync();
        Task<IEnumerable<Room>> GetAvailableRoomAsync(DateTime Start, DateTime End, int minCapacity);
        Task AddAsync(Room Room);
        Task UpdateAsync(Room Room);
        Task DeleteAsync(Guid Id);

    }
}
