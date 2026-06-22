using ConferenceRoomsAPI.Domain.Entities;
using ConferenceRoomsAPI.Domain.Enums;
using ConferenceRoomsAPI.Infrastructure.Database;
using ConferenceRoomsAPI.Infrastructure.InterfacesOfRepositories;
using Microsoft.EntityFrameworkCore;

namespace ConferenceRoomsAPI.Infrastructure.Repositories
{
    public class RoomRepository : IRoomRepository
    {
        private readonly AppDbContext _appDbContext;

        public RoomRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task AddAsync(Room Room)
        {
            await _appDbContext.Rooms.AddAsync(Room);

            await _appDbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid Id)
        {
            var room = await GetByIdAsync(Id);
            if (room != null)
                return;

            // Soft delete
            room.IsDeleted = true;
            room.UpdatedAt = DateTime.UtcNow;
            await _appDbContext.SaveChangesAsync();
        }
        public async Task UpdateAsync(Room Room)
        {
            Room.UpdatedAt = DateTime.UtcNow;
            _appDbContext.Rooms.Update(Room);

            await _appDbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Room>> GetAllAsync()
        {
            return await _appDbContext.Rooms.Include(r => r.RoomServices).ThenInclude(rs => rs.Service).ToListAsync();
        }


        // Returns Rooms that - have enough capacity; have no confirmed bookings that overlap with the requested time slot
        public async Task<IEnumerable<Room>> GetAvailableRoomAsync(DateTime Start, DateTime End, int minCapacity)
        {
            return await _appDbContext.Rooms.Where(r => r.Capacity >= minCapacity)
                .Where(r => !r.Bookings.Any(b => b.Status == BookingStatus.Confirmed && b.StartTime < End && b.EndTime > Start))
                .Include(r => r.RoomServices)
                .ThenInclude(rs => rs.Service)
                .ToListAsync();
        }

        public async Task<Room?> GetByIdAsync(Guid Id)
        {
            // HasQueryFilter auto add WHERE IsDeleted = false
            return await _appDbContext.Rooms.FindAsync(Id);
        }

        public async Task<Room?> GetByIdWithServicesAsync(Guid Id)
        {
            return await _appDbContext.Rooms.Include(r => r.RoomServices).ThenInclude(rs => rs.Service).FirstOrDefaultAsync(r => r.Id == Id);
        }

    }
}
