using HotelBookingAPI.DTOs;

namespace HotelBookingAPI.Interfaces
{
    public interface IRoomService
    {
        IEnumerable<RoomDTO> GetAllRooms();
        RoomDTO? GetRoomById(int id);
        RoomDTO CreateRoom(CreateRoomDTO createRoomDto);
        RoomDTO? UpdateRoom(int id, UpdateRoomDTO updateRoomDto);
        bool DeleteRoom(int id);
        IEnumerable<RoomDTO> GetAvailableRooms(RoomAvailabilityRequestDTO request);
    }
}
