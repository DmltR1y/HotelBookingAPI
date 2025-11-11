using AutoMapper;
using HotelBookingAPI.DTOs;
using HotelBookingAPI.Interfaces;
using HotelBookingAPI.Models;

namespace HotelBookingAPI.Services
{
    public class BookingService : IBookingService
    {
        private IBookingRepository _bookingRepository;
        private IRoomRepository _roomRepository;
        private IGuestRepository _guestRepository;
        private IMapper _mapper;

        public BookingService(
            IBookingRepository bookingRepository,
            IRoomRepository roomRepository,
            IGuestRepository guestRepository,
            IMapper mapper)
        {
            _bookingRepository = bookingRepository;
            _roomRepository = roomRepository;
            _guestRepository = guestRepository;
            _mapper = mapper;
        }

        public IEnumerable<BookingDTO> GetAllBookings()
        {
            var bookings = _bookingRepository.GetAllBookings();
            return _mapper.Map<IEnumerable<BookingDTO>>(bookings);
        }

        public BookingDTO? GetBookingById(int id)
        {
            var booking = _bookingRepository.GetBookingById(id);
            return _mapper.Map<BookingDTO>(booking);
        }

        public BookingResponseDTO CreateBooking(CreateBookingDTO createBookingDto)
        {
            var response = new BookingResponseDTO();

            // Валидация данных
            if (createBookingDto.CheckInDate >= createBookingDto.CheckOutDate)
            {
                response.Success = false;
                response.Errors.Add("Дата выезда должна быть позже даты заезда");
                return response;
            }

            if (createBookingDto.CheckInDate < DateTime.Today)
            {
                response.Success = false;
                response.Errors.Add("Дата заезда не может быть в прошлом");
                return response;
            }

            // Проверяем существование гостя
            var guest = _guestRepository.GetGuestById(createBookingDto.GuestId);
            if (guest == null)
            {
                response.Success = false;
                response.Errors.Add("Гость не найден");
                return response;
            }

            // Проверяем существование комнаты
            var room = _roomRepository.GetRoomById(createBookingDto.RoomId);
            if (room == null)
            {
                response.Success = false;
                response.Errors.Add("Комната не найдена");
                return response;
            }

            // Проверяем доступность комнаты
            var isRoomAvailable = _bookingRepository.IsRoomAvailable(
                createBookingDto.RoomId,
                createBookingDto.CheckInDate,
                createBookingDto.CheckOutDate);

            if (!isRoomAvailable)
            {
                response.Success = false;
                response.Errors.Add("Комната уже забронирована на указанные даты");
                return response;
            }

            // Проверяем вместимость
            if (createBookingDto.NumberOfGuests > room.Capacity)
            {
                response.Success = false;
                response.Errors.Add($"Комната вмещает только {room.Capacity} гостей");
                return response;
            }

            // Создаем бронирование
            var booking = _mapper.Map<Booking>(createBookingDto);
            booking.TotalPrice = CalculateTotalPrice(room.PricePerNight, createBookingDto.CheckInDate, createBookingDto.CheckOutDate);
            booking.Status = "Confirmed";

            var createdBooking = _bookingRepository.AddBooking(booking);

            response.Success = true;
            response.Message = "Бронирование успешно создано";
            response.Booking = _mapper.Map<BookingDTO>(createdBooking);

            return response;
        }

        public BookingResponseDTO UpdateBookingStatus(int id, UpdateBookingStatusDTO updateStatusDto)
        {
            var response = new BookingResponseDTO();

            var booking = _bookingRepository.GetBookingById(id);
            if (booking == null)
            {
                response.Success = false;
                response.Errors.Add("Бронирование не найдено");
                return response;
            }

            // Обновляем статус
            booking.Status = updateStatusDto.Status;

            // Если статус "CheckedIn", устанавливаем время заезда
            if (updateStatusDto.Status == "CheckedIn")
            {
                booking.CheckedInAt = DateTime.UtcNow;
            }
            // Если статус "CheckedOut", устанавливаем время выезда
            else if (updateStatusDto.Status == "CheckedOut")
            {
                booking.CheckedOutAt = DateTime.UtcNow;
            }

            var updatedBooking = _bookingRepository.UpdateBooking(id, booking);

            response.Success = true;
            response.Message = $"Статус бронирования изменен на: {updateStatusDto.Status}";
            response.Booking = _mapper.Map<BookingDTO>(updatedBooking);

            return response;
        }

        public bool CancelBooking(int id)
        {
            var booking = _bookingRepository.GetBookingById(id);
            if (booking == null)
                return false;

            // Меняем статус на "Cancelled" вместо удаления
            booking.Status = "Cancelled";
            _bookingRepository.UpdateBooking(id, booking);
            return true;
        }

        public IEnumerable<BookingDTO> GetBookingsByGuestId(int guestId)
        {
            var bookings = _bookingRepository.GetBookingsByGuestId(guestId);
            return _mapper.Map<IEnumerable<BookingDTO>>(bookings);
        }

        public IEnumerable<BookingDTO> GetTodayCheckIns()
        {
            var allBookings = _bookingRepository.GetAllBookings();
            var todayCheckIns = allBookings
                .Where(b => b.CheckInDate.Date == DateTime.Today && b.Status == "Confirmed")
                .ToList();

            return _mapper.Map<IEnumerable<BookingDTO>>(todayCheckIns);
        }

        public IEnumerable<BookingDTO> GetTodayCheckOuts()
        {
            var allBookings = _bookingRepository.GetAllBookings();
            var todayCheckOuts = allBookings
                .Where(b => b.CheckOutDate.Date == DateTime.Today &&
                           (b.Status == "Confirmed" || b.Status == "CheckedIn"))
                .ToList();

            return _mapper.Map<IEnumerable<BookingDTO>>(todayCheckOuts);
        }

        private decimal CalculateTotalPrice(decimal pricePerNight, DateTime checkIn, DateTime checkOut)
        {
            var nights = (checkOut - checkIn).Days;
            return pricePerNight * nights;
        }
    }
}
