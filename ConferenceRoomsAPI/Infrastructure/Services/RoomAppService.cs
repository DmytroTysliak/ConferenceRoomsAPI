using ConferenceRoomsAPI.Domain.Entities;
using ConferenceRoomsAPI.Domain.Services;
using ConferenceRoomsAPI.Infrastructure.InterfacesOfRepositories;
using System.Data;
using static ConferenceRoomsAPI.Domain.DTOs.CreateRoomDto;
using static ConferenceRoomsAPI.Domain.DTOs.ServiceDto;

namespace ConferenceRoomsAPI.Infrastructure.Services
{
    // Handles all business operations related to conference rooms.
    public class RoomAppService
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IServiceRepository _serviceRepository;

        public RoomAppService(IRoomRepository roomRepository, IServiceRepository serviceRepository)
        {
            _roomRepository = roomRepository;
            _serviceRepository = serviceRepository;
        }

        // Creates a new room and links it to the requested services
        public async Task<RoomDetailResponse> CreateRoomAsync(CreateRoomRequest request)
        {
            var services = await _serviceRepository.GetByIdsAsync(request.ServiceIds);
            var foundIds = services.Select(s => s.Id).ToHashSet();
            var missingIds = request.ServiceIds.Where(Id => !foundIds.Contains(Id)).ToList();

            if (missingIds.Any())
                throw new KeyNotFoundException($"Services not found: {string.Join(", ", missingIds)}");

            var room = new Room
            {
                Name = request.Name,
                Capacity = request.Capacity,
                HourlyPrice = request.HourlyPrice,
                RoomServices = request.ServiceIds.Select(id => new RoomService { ServiceId = id }).ToList(),
            };

            await _roomRepository.AddAsync(room);

            return await GetRoomByIdAsync(room.Id)
                ?? throw new InvalidOperationException("Failed to retrieve created room.");
        }

        // Returns rooms available for the requested time slot and capacity
        public async Task<RoomDetailResponse?> GetRoomByIdAsync(Guid id)
        {
            var room = await _roomRepository.GetByIdWithServicesAsync(id);
            return room is null ? null : MapToDetail(room);
        }

        // Update room fields. Only non-null fields in the request are changed
        public async Task UpdateRoomAsync(Guid id, UpdateRoomRequest request)
        {
            var room =  await _roomRepository.GetByIdWithServicesAsync(id)
                ?? throw new KeyNotFoundException($"Room {id} not found!");

            if (request.Name is not null)
                room.Name = request.Name;

            if (request.Capacity is not null)
                room.Capacity = request.Capacity.Value;

            if (request.HourlyPrice is not null)
                room.HourlyPrice = request.HourlyPrice.Value;

            if (request.ServiceIds is not null)
            {
                var services = await _serviceRepository.GetByIdsAsync(request.ServiceIds);

                room.RoomServices = request.ServiceIds.Select(sid => new RoomService { RoomId = room.Id, ServiceId = sid }).ToList();
            }

            await _roomRepository.UpdateAsync(room);
        }

        public async Task DeleteRoomAsync(Guid id)
        {
            var room = await _roomRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Room {id} not found!");

            await _roomRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<RoomSummaryResponse>> GetAllRoomsAsync()
        {
            var rooms = await _roomRepository.GetAllAsync();
            return rooms.Select(MapToSummary);
        } 

        public async Task<IEnumerable<RoomSummaryResponse>> GetAvailableRoomsAsync(RoomAvailabilityRequest request)
        {
            if (request.StartTime >= request.EndTime)
                throw new ArgumentException("Start time must bebefore the end time.");

            var rooms = await _roomRepository.GetAvailableRoomAsync(request.StartTime, request.EndTime, request.MinCapacity);

            return rooms.Select(MapToSummary);
        }

        public static RoomDetailResponse MapToDetail(Room room) => new(
            room.Id,
            room.Name,
            room.Capacity,
            room.HourlyPrice,
            room.RoomServices.Select(rs => new ServiceResponse(rs.ServiceId, rs.Service.Name, rs.Service.Price)).ToList(),
            room.CreatedAt,
            room.UpdatedAt
            );

        public static RoomSummaryResponse MapToSummary(Room room) => new(
            room.Id,
            room.Name,
            room.Capacity,
            room.HourlyPrice
            );



    }
}
