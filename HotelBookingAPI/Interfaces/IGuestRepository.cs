using HotelBookingAPI.Models;

namespace HotelBookingAPI.Interfaces
{
    public interface IGuestRepository
    {
        IEnumerable<Guest> GetAllGuests();
        Guest? GetGuestById(int id);
        Guest AddGuest(Guest guest);
        Guest? UpdateGuest(int id, Guest guest);
        Guest? DeleteGuest(int id);
        bool GuestExists(int id);
        IEnumerable<Guest> SearchGuests(string searchTerm);
    }
}
