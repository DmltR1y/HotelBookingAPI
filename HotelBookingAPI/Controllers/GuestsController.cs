using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HotelBookingAPI.DTOs;
using HotelBookingAPI.Interfaces;

namespace HotelBookingAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class GuestsController : ControllerBase
    {
        private readonly IGuestService _guestService;

        public GuestsController(IGuestService guestService)
        {
            _guestService = guestService;
        }

        // GET: api/guests
        [HttpGet]
        [Authorize(Roles = "Admin,Manager,Receptionist")]
        public ActionResult<IEnumerable<GuestDTO>> GetGuests()
        {
            var guests = _guestService.GetAllGuests();
            return Ok(guests);
        }

        // GET api/guests/id
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Manager,Receptionist")]
        public ActionResult<GuestDTO> GetGuest(int id)
        {
            var guest = _guestService.GetGuestById(id);

            if (guest == null)
            {
                return NotFound($"Гость с ID {id} не найден");
            }

            return Ok(guest);
        }

        // GET api/guests/search
        [HttpGet("search")]
        [Authorize(Roles = "Admin,Manager,Receptionist")]
        public ActionResult<IEnumerable<GuestDTO>> SearchGuests([FromQuery] string term)
        {
            try
            {
                var guests = _guestService.SearchGuests(term);
                return Ok(guests);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST api/guests
        [HttpPost]
        [Authorize(Roles = "Admin,Manager,Receptionist")]
        public ActionResult<GuestDTO> CreateGuest(CreateGuestDTO createGuestDto)
        {
            var guest = _guestService.CreateGuest(createGuestDto);
            return CreatedAtAction(nameof(GetGuest), new { id = guest.GuestId }, guest);
        }

        // PUT api/guests/id
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager,Receptionist")]
        public ActionResult<GuestDTO> UpdateGuest(int id, UpdateGuestDTO updateGuestDto)
        {
            var guest = _guestService.UpdateGuest(id, updateGuestDto);

            if (guest == null)
            {
                return NotFound($"Гость с ID {id} не найден");
            }

            return Ok(guest);
        }

        // DELETE api/guests/id
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public ActionResult DeleteGuest(int id)
        {
            var result = _guestService.DeleteGuest(id);

            if (!result)
            {
                return NotFound($"Гость с ID {id} не найден");
            }

            return NoContent();
        }
    }
}