using HotelBookingAPI.Models;

namespace HotelBookingAPI.Interfaces
{
    public interface IRoomRepository
    {
        IEnumerable<Room> GetAllRooms();
        Room? GetRoomById(int id);
        Room AddRoom(Room room);
        Room? UpdateRoom(int id, Room room);
        Room? DeleteRoom(int id);
        bool RoomExists(int id);
        bool RoomNumberExists(string roomNumber);
        IEnumerable<Room> GetAvailableRooms(DateTime checkIn, DateTime checkOut);
    }
}
