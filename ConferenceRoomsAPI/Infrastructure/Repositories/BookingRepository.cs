using ConferenceRoomsAPI.Domain.Entities;
using ConferenceRoomsAPI.Domain.Enums;
using ConferenceRoomsAPI.Infrastructure.Database;
using ConferenceRoomsAPI.Infrastructure.InterfacesOfRepositories;
using Microsoft.EntityFrameworkCore;

namespace ConferenceRoomsAPI.Infrastructure.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly AppDbContext _appDbContext;

        public BookingRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task AddAsync(Booking Booking)
        {
            await _appDbContext.Bookings.AddAsync(Booking);
            await _appDbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Booking>> GetAllAsync()
        {
            return await _appDbContext.Bookings.Include(b => b.Room).Include(b => b.BookingServices)
                .ThenInclude(bs => bs.Service).OrderByDescending(b => b.StartTime).ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetByDateRangeAsync(DateTime From, DateTime To)
        {
            return await _appDbContext.Bookings.Where(b => b.StartTime >= From && b.EndTime <= To).Include(b => b.Room)
                .Include(b => b.BookingServices).ThenInclude(bs => bs.Service).ToListAsync();
        }

        public async Task<Booking?> GetByIdAsync(Guid Id)
        {
            return await _appDbContext.Bookings.FindAsync(Id);
        }

        public async Task<Booking?> GetByIdWithDetailsAsync(Guid Id)
        {
            return await _appDbContext.Bookings.Include(b => b.Room).Include(b => b.BookingServices).FirstOrDefaultAsync(b => b.Id == Id);
        }

        public async Task<IEnumerable<Booking>> GetByRoomIdAsync(Guid RoomId)
        {
            return await _appDbContext.Bookings.Where(b => b.RoomId == RoomId).OrderByDescending(b => b.StartTime).ToListAsync();
        }

        public async Task<bool> HasOverlappingBookingAsync(Guid RoomId, DateTime Start, DateTime End)
        {
            return await _appDbContext.Bookings.AnyAsync(b =>
                b.RoomId == RoomId &&
                b.Status == BookingStatus.Confirmed &&
                b.StartTime < End &&
                b.EndTime > Start);
        }

        public async Task UpdateAsync(Booking Booking)
        {
            _appDbContext.Bookings.Update(Booking);
            await _appDbContext.SaveChangesAsync();
        }
    }
}
