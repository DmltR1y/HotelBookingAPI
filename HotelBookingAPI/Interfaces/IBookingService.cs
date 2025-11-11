using HotelBookingAPI.DTOs;

namespace HotelBookingAPI.Interfaces
{
    public interface IBookingService
    {
        IEnumerable<BookingDTO> GetAllBookings();
        BookingDTO? GetBookingById(int id);
        BookingResponseDTO CreateBooking(CreateBookingDTO createBookingDto);
        BookingResponseDTO UpdateBookingStatus(int id, UpdateBookingStatusDTO updateStatusDto);
        bool CancelBooking(int id);
        IEnumerable<BookingDTO> GetBookingsByGuestId(int guestId);
        IEnumerable<BookingDTO> GetTodayCheckIns();
        IEnumerable<BookingDTO> GetTodayCheckOuts();
    }
}