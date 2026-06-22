using ConferenceRoomsAPI.Domain.Entities;
using ConferenceRoomsAPI.Infrastructure.Database;
using ConferenceRoomsAPI.Infrastructure.InterfacesOfRepositories;
using Microsoft.EntityFrameworkCore;

namespace ConferenceRoomsAPI.Infrastructure.Repositories
{
    public class ServiceRepository : IServiceRepository
    {
        private readonly AppDbContext _appDbContext;

        public ServiceRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }


        public async Task AddAsync(Service Service)
        {
            await _appDbContext.AddAsync(Service);
            await _appDbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid Id)
        {
            var service = await GetByIdAsync(Id);
            if (service != null)
                return;

            service.IsDeleted = true;
            await _appDbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Service>> GetAllAsync()
        {
            return await _appDbContext.Services.ToListAsync();
        }

        public async Task<Service?> GetByIdAsync(Guid Id)
        {
            return await _appDbContext.Services.FindAsync(Id);
        }

        // Fetches multiple services by their IDs in one query. Used when creating a booking — validates that all requested services exist.
        public async Task<IEnumerable<Service>> GetByIdsAsync(IEnumerable<Guid> Ids)
        {
            return await _appDbContext.Services.Where(s => Ids.Contains(s.Id)).ToListAsync();
        }

        public async Task UpdateAsync(Service Service)
        {
            _appDbContext.Services.Update(Service);
            await _appDbContext.SaveChangesAsync();
        }
    }
}
