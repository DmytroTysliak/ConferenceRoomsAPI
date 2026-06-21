using ConferenceRoomsAPI.Domain.Entities;

namespace ConferenceRoomsAPI.Infrastructure.InterfacesOfRepositories
{
    public interface IServiceRepository
    {
        Task<Service?> GetByIdAsync(Guid Id);
        Task<IEnumerable<Service>> GetAllAsync();
        Task<IEnumerable<Service>> GetByIdsAsync(IEnumerable<Guid> Ids);
        Task AddAsync(Service Service);
        Task UpdateAsync(Service Service);
        Task DeleteAsync(Guid Id);
    }
}
