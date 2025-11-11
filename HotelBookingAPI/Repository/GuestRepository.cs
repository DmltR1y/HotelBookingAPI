using HotelBookingAPI.Data;
using HotelBookingAPI.Interfaces;
using HotelBookingAPI.Models;

namespace HotelBookingAPI.Repository
{
    public class GuestRepository : IGuestRepository
    {
        private readonly ApplicationDbContext _context;

        public GuestRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Guest> GetAllGuests()
        {
            return _context.Guests
                .Where(g => g.IsActive)
                .ToList();
        }

        public Guest? GetGuestById(int id)
        {
            return _context.Guests
                .FirstOrDefault(g => g.GuestId == id && g.IsActive);
        }

        public Guest AddGuest(Guest guest)
        {
            _context.Guests.Add(guest);
            _context.SaveChanges();
            return guest;
        }

        public Guest? UpdateGuest(int id, Guest guest)
        {
            var existingGuest = _context.Guests.Find(id);
            if (existingGuest == null || !existingGuest.IsActive)
                return null;

            // Обновляем поля
            existingGuest.FirstName = guest.FirstName;
            existingGuest.LastName = guest.LastName;
            existingGuest.Email = guest.Email;
            existingGuest.Phone = guest.Phone;
            existingGuest.PassportNumber = guest.PassportNumber;
            existingGuest.DateOfBirth = guest.DateOfBirth;
            existingGuest.Address = guest.Address;

            _context.SaveChanges();
            return existingGuest;
        }

        public Guest? DeleteGuest(int id)
        {
            var guest = _context.Guests.Find(id);
            if (guest == null)
                return null;

            // Мягкое удаление
            guest.IsActive = false;
            _context.SaveChanges();
            return guest;
        }

        public bool GuestExists(int id)
        {
            return _context.Guests.Any(g => g.GuestId == id && g.IsActive);
        }

        public IEnumerable<Guest> SearchGuests(string searchTerm)
        {
            return _context.Guests
                .Where(g => g.IsActive &&
                           (g.FirstName.Contains(searchTerm) ||
                            g.LastName.Contains(searchTerm) ||
                            g.Email.Contains(searchTerm) ||
                            g.Phone.Contains(searchTerm) ||
                            g.PassportNumber.Contains(searchTerm)))
                .ToList();
        }
    }
}
