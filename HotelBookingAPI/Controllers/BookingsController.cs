using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HotelBookingAPI.DTOs;
using HotelBookingAPI.Interfaces;

namespace HotelBookingAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        // GET: api/bookings
        [HttpGet]
        [Authorize(Roles = "Admin,Manager,Receptionist")]
        public ActionResult<IEnumerable<BookingDTO>> GetBookings()
        {
            var bookings = _bookingService.GetAllBookings();
            return Ok(bookings);
        }

        // GET api/bookings/id
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Manager,Receptionist")]
        public ActionResult<BookingDTO> GetBooking(int id)
        {
            var booking = _bookingService.GetBookingById(id);

            if (booking == null)
            {
                return NotFound($"Бронирование с ID {id} не найдено");
            }

            return Ok(booking);
        }

        // GET api/bookings/guest/id
        [HttpGet("guest/{guestId}")]
        [Authorize(Roles = "Admin,Manager,Receptionist")]
        public ActionResult<IEnumerable<BookingDTO>> GetBookingsByGuest(int guestId)
        {
            var bookings = _bookingService.GetBookingsByGuestId(guestId);
            return Ok(bookings);
        }

        // GET api/bookings/today/checkins
        [HttpGet("today/checkins")]
        [Authorize(Roles = "Admin,Manager,Receptionist")]
        public ActionResult<IEnumerable<BookingDTO>> GetTodayCheckIns()
        {
            var checkIns = _bookingService.GetTodayCheckIns();
            return Ok(checkIns);
        }

        // GET api/bookings/today/checkouts
        [HttpGet("today/checkouts")]
        [Authorize(Roles = "Admin,Manager,Receptionist")]
        public ActionResult<IEnumerable<BookingDTO>> GetTodayCheckOuts()
        {
            var checkOuts = _bookingService.GetTodayCheckOuts();
            return Ok(checkOuts);
        }

        // POST api/bookings
        [HttpPost]
        [Authorize(Roles = "Admin,Manager,Receptionist")]
        public ActionResult<BookingResponseDTO> CreateBooking(CreateBookingDTO createBookingDto)
        {
            var response = _bookingService.CreateBooking(createBookingDto);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return CreatedAtAction(nameof(GetBooking), new { id = response.Booking?.BookingId }, response);
        }

        // PUT api/bookings/id/status
        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin,Manager,Receptionist")]
        public ActionResult<BookingResponseDTO> UpdateBookingStatus(int id, UpdateBookingStatusDTO updateStatusDto)
        {
            var response = _bookingService.UpdateBookingStatus(id, updateStatusDto);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        // DELETE api/bookings/id
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Manager,Receptionist")]
        public ActionResult CancelBooking(int id)
        {
            var result = _bookingService.CancelBooking(id);

            if (!result)
            {
                return NotFound($"Бронирование с ID {id} не найдено");
            }

            return NoContent();
        }
    }
}