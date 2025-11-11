using Microsoft.EntityFrameworkCore;
using HotelBookingAPI.Data;
using HotelBookingAPI.Interfaces;
using HotelBookingAPI.Models;

namespace HotelBookingAPI.Repository
{
    public class RoomRepository : IRoomRepository
    {
        private readonly ApplicationDbContext _context;

        public RoomRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Room> GetAllRooms()
        {
            return _context.Rooms
                .Include(r => r.RoomType)
                .ToList();
        }

        public Room? GetRoomById(int id)
        {
            return _context.Rooms
                .Include(r => r.RoomType)
                .FirstOrDefault(r => r.RoomId == id);
        }

        public Room AddRoom(Room room)
        {
            _context.Rooms.Add(room);
            _context.SaveChanges();
            return room;
        }

        public Room? UpdateRoom(int id, Room room)
        {
            var existingRoom = _context.Rooms.Find(id);
            if (existingRoom == null)
                return null;

            // Обновляем поля
            existingRoom.RoomNumber = room.RoomNumber;
            existingRoom.RoomTypeId = room.RoomTypeId;
            existingRoom.PricePerNight = room.PricePerNight;
            existingRoom.Capacity = room.Capacity;
            existingRoom.Description = room.Description;
            existingRoom.IsAvailable = room.IsAvailable;

            _context.SaveChanges();
            return existingRoom;
        }

        public Room? DeleteRoom(int id)
        {
            var room = _context.Rooms.Find(id);
            if (room == null)
                return null;

            _context.Rooms.Remove(room);
            _context.SaveChanges();
            return room;
        }

        public bool RoomExists(int id)
        {
            return _context.Rooms.Any(r => r.RoomId == id);
        }

        public bool RoomNumberExists(string roomNumber)
        {
            return _context.Rooms.Any(r => r.RoomNumber == roomNumber);
        }

        public IEnumerable<Room> GetAvailableRooms(DateTime checkIn, DateTime checkOut)
        {
            var sql = @"
        SELECT r.* 
        FROM Rooms r
        WHERE r.IsAvailable = 1 
        AND r.RoomId NOT IN (
            SELECT DISTINCT b.RoomId 
            FROM Bookings b 
            WHERE b.Status != 'Cancelled' 
            AND b.CheckInDate < {0} 
            AND b.CheckOutDate > {1}
        )";

            var availableRooms = _context.Rooms
                .FromSqlRaw(sql, checkOut, checkIn)
                .Include(r => r.RoomType)
                .ToList();

            return availableRooms;
        }
    }
}
