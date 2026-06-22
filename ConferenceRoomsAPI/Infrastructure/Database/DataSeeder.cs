using ConferenceRoomsAPI.Domain.Entities;
using ConferenceRoomsAPI.Domain.Services;
using Microsoft.EntityFrameworkCore;

namespace ConferenceRoomsAPI.Infrastructure.Database
{
    public class DataSeeder
    {
        private readonly AppDbContext _appDbContext;

        public DataSeeder(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task SeedAsync()
        {
            await _appDbContext.Database.MigrateAsync();

            if (await _appDbContext.Services.AnyAsync())
                return;

            // Services
            var projector = new Service { Name = "Проєктор", Price = 500 };
            var wifi = new Service { Name = "Wi-Fi", Price = 300 };
            var sound = new Service { Name = "Звук", Price = 700 };

            await _appDbContext.Services.AddRangeAsync(projector, wifi, sound);


            // Rooms
            var roomA = new Room { Name = "Зал А", Capacity = 50, HourlyPrice = 2000 };
            var roomB = new Room { Name = "Зал B", Capacity = 100, HourlyPrice = 3500 };
            var roomC = new Room { Name = "Зал C", Capacity = 30, HourlyPrice = 1500 };

            await _appDbContext.Rooms.AddRangeAsync(roomA, roomB, roomC);
            await _appDbContext.SaveChangesAsync();

            // Room <-> Service connections
            var roomServices = new List<RoomService>
            {
                new() { RoomId = roomA.Id, ServiceId = projector.Id},
                new() { RoomId = roomA.Id, ServiceId = wifi.Id },
                new() { RoomId = roomA.Id, ServiceId = sound.Id },

                new() { RoomId = roomB.Id, ServiceId = projector.Id },
                new() { RoomId = roomB.Id, ServiceId = wifi.Id },
                new() { RoomId = roomB.Id, ServiceId = sound.Id },

                new() { RoomId = roomC.Id, ServiceId = projector.Id },
                new() { RoomId = roomC.Id, ServiceId = wifi.Id },
                new() { RoomId = roomC.Id, ServiceId = sound.Id },
            };

            await _appDbContext.RoomServices.AddRangeAsync(roomServices);
            await _appDbContext.SaveChangesAsync();
        }
    }
}
