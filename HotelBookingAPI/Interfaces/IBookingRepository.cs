using HotelBookingAPI.Models;

namespace HotelBookingAPI.Interfaces
{
    public interface IBookingRepository
    {
        IEnumerable<Booking> GetAllBookings();
        Booking? GetBookingById(int id);
        Booking AddBooking(Booking booking);
        Booking? UpdateBooking(int id, Booking booking);
        Booking? DeleteBooking(int id);
        bool BookingExists(int id);
        IEnumerable<Booking> GetBookingsByGuestId(int guestId);
        bool IsRoomAvailable(int roomId, DateTime checkIn, DateTime checkOut, int? excludeBookingId = null);
    }
}
