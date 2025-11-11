using AutoMapper;
using HotelBookingAPI.DTOs;
using HotelBookingAPI.Interfaces;
using HotelBookingAPI.Models;

namespace HotelBookingAPI.Services
{
    public class GuestService : IGuestService
    {
        private IGuestRepository _guestRepository;
        private IMapper _mapper;

        public GuestService(IGuestRepository guestRepository, IMapper mapper)
        {
            _guestRepository = guestRepository;
            _mapper = mapper;
        }

        public IEnumerable<GuestDTO> GetAllGuests()
        {
            var guests = _guestRepository.GetAllGuests();
            return _mapper.Map<IEnumerable<GuestDTO>>(guests);
        }

        public GuestDTO? GetGuestById(int id)
        {
            var guest = _guestRepository.GetGuestById(id);
            return _mapper.Map<GuestDTO>(guest);
        }

        public GuestDTO CreateGuest(CreateGuestDTO createGuestDto)
        {
            var guest = _mapper.Map<Guest>(createGuestDto);
            var createdGuest = _guestRepository.AddGuest(guest);
            return _mapper.Map<GuestDTO>(createdGuest);
        }

        public GuestDTO? UpdateGuest(int id, UpdateGuestDTO updateGuestDto)
        {
            var guest = _mapper.Map<Guest>(updateGuestDto);
            var updatedGuest =_guestRepository.UpdateGuest(id, guest);
            return _mapper.Map<GuestDTO>(updatedGuest);
        }

        public bool DeleteGuest(int id)
        {
            var guest = _guestRepository.DeleteGuest(id);
            return guest != null;
        }

        public IEnumerable<GuestDTO> SearchGuests(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm) || searchTerm.Length < 2)
            {
                throw new ArgumentException("Поисковый запрос должен содержать минимум 2 символа");
            }

            var guests = _guestRepository.SearchGuests(searchTerm);
            return _mapper.Map<IEnumerable<GuestDTO>>(guests);
        }
    }
}
