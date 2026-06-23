using ConferenceRoomsAPI.Domain.Entities;
using ConferenceRoomsAPI.Infrastructure.InterfacesOfRepositories;
using static ConferenceRoomsAPI.Domain.DTOs.ServiceDto;

namespace ConferenceRoomsAPI.Infrastructure.Services
{
    // Handles CRUD operations for additional services like projector, Wi-Fi or sound
    public class ServiceAppService
    {
        private readonly IServiceRepository _repository;

        public ServiceAppService(IServiceRepository repository)
        {
            _repository = repository;
        }

        public async Task<ServiceResponse> CreateServiceAsync (CreateServiceRequest request)
        {
            var service = new Service
            {
                Name = request.Name,
                Price = request.Price
            };

            await _repository.AddAsync(service);

            return new ServiceResponse(service.Id, service.Name, service.Price);
        }

        public async Task UpdateServiceAsync(Guid id, UpdateServiceRequest request)
        {
            var service = await _repository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Service {id} not found!");

            if (request.Name is not null)
                service.Name = request.Name;

            if (request.Price is not null)
                service.Price = request.Price.Value;

            await _repository.UpdateAsync(service);
        }

        public async Task DeleteServiceAsync(Guid id)
        {
            var service = await _repository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Service {id} not found!");

            await _repository.DeleteAsync(id);
        }

        public async Task<ServiceResponse?> GetByIdAsync(Guid id)
        {
            var service = await _repository.GetByIdAsync(id);

            return service is null ? null : new ServiceResponse(service.Id,service.Name,service.Price);
        }

        public async Task<IEnumerable<ServiceResponse>> GetAllAsync()
        {
            var service = await _repository.GetAllAsync();

            return service.Select(s => new ServiceResponse(s.Id, s.Name, s.Price));
        }
    }
}
