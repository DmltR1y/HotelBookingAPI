using HotelBookingAPI.DTOs;

namespace HotelBookingAPI.Interfaces
{
    public interface IGuestService
    {
        IEnumerable<GuestDTO> GetAllGuests();
        GuestDTO? GetGuestById(int id);
        GuestDTO CreateGuest(CreateGuestDTO createGuestDto);
        GuestDTO? UpdateGuest(int id, UpdateGuestDTO updateGuestDto);
        bool DeleteGuest(int id);
        IEnumerable<GuestDTO> SearchGuests(string searchTerm);
    }
}
