using Microsoft.EntityFrameworkCore;
using HotelBookingAPI.Data;
using HotelBookingAPI.Interfaces;
using HotelBookingAPI.Models;

namespace HotelBookingAPI.Repository
{
    public class BookingRepository : IBookingRepository
    {
        private readonly ApplicationDbContext _context;

        public BookingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Booking> GetAllBookings()
        {
            return _context.Bookings
                .Include(b => b.Guest)
                .Include(b => b.Room)
                    .ThenInclude(r => r.RoomType)
                .ToList();
        }

        public Booking? GetBookingById(int id)
        {
            return _context.Bookings
                .Include(b => b.Guest)
                .Include(b => b.Room)
                    .ThenInclude(r => r.RoomType)
                .FirstOrDefault(b => b.BookingId == id);
        }

        public Booking AddBooking(Booking booking)
        {
            _context.Bookings.Add(booking);
            _context.SaveChanges();
            return booking;
        }

        public Booking? UpdateBooking(int id, Booking booking)
        {
            var existingBooking = _context.Bookings.Find(id);
            if (existingBooking == null)
                return null;

            // Обновляем поля
            existingBooking.GuestId = booking.GuestId;
            existingBooking.RoomId = booking.RoomId;
            existingBooking.CheckInDate = booking.CheckInDate;
            existingBooking.CheckOutDate = booking.CheckOutDate;
            existingBooking.NumberOfGuests = booking.NumberOfGuests;
            existingBooking.TotalPrice = booking.TotalPrice;
            existingBooking.Status = booking.Status;
            existingBooking.SpecialRequests = booking.SpecialRequests;

            _context.SaveChanges();
            return existingBooking;
        }

        public Booking? DeleteBooking(int id)
        {
            var booking = _context.Bookings.Find(id);
            if (booking == null)
                return null;

            _context.Bookings.Remove(booking);
            _context.SaveChanges();
            return booking;
        }

        public bool BookingExists(int id)
        {
            return _context.Bookings.Any(b => b.BookingId == id);
        }

        public IEnumerable<Booking> GetBookingsByGuestId(int guestId)
        {
            return _context.Bookings
                .Include(b => b.Guest)
                .Include(b => b.Room)
                    .ThenInclude(r => r.RoomType)
                .Where(b => b.GuestId == guestId)
                .ToList();
        }

        public bool IsRoomAvailable(int roomId, DateTime checkIn, DateTime checkOut, int? excludeBookingId = null)
        {
            var query = _context.Bookings
                .Where(b => b.RoomId == roomId &&
                           b.Status != "Cancelled" &&
                           (b.CheckInDate < checkOut && b.CheckOutDate > checkIn));

            if (excludeBookingId.HasValue)
            {
                query = query.Where(b => b.BookingId != excludeBookingId.Value);
            }

            return ! query.Any();
        }
    }
}
