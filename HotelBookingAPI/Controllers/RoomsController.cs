using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HotelBookingAPI.DTOs;
using HotelBookingAPI.Interfaces;

namespace HotelBookingAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RoomsController : ControllerBase
    {
        private readonly IRoomService _roomService;

        public RoomsController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        // GET: api/rooms
        [HttpGet]
        [AllowAnonymous]
        public ActionResult<IEnumerable<RoomDTO>> GetRooms()
        {
            var rooms = _roomService.GetAllRooms();
            return Ok(rooms);
        }

        // GET api/rooms/id
        [HttpGet("{id}")]
        [AllowAnonymous]
        public ActionResult<RoomDTO> GetRoom(int id)
        {
            var room = _roomService.GetRoomById(id);

            if (room == null)
            {
                return NotFound($"Комната с ID {id} не найдена");
            }

            return Ok(room);
        }

        // GET api/rooms/available
        [HttpGet("available")]
        [AllowAnonymous]
        public ActionResult<IEnumerable<RoomDTO>> GetAvailableRooms(
            [FromQuery] DateTime checkIn,
            [FromQuery] DateTime checkOut)
        {
            try
            {
                var request = new RoomAvailabilityRequestDTO
                {
                    CheckInDate = checkIn,
                    CheckOutDate = checkOut
                };

                var rooms = _roomService.GetAvailableRooms(request);
                return Ok(rooms);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST api/rooms
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public ActionResult<RoomDTO> CreateRoom(CreateRoomDTO createRoomDto)
        {
            try
            {
                var room = _roomService.CreateRoom(createRoomDto);
                return CreatedAtAction(nameof(GetRoom), new { id = room.RoomId }, room);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT api/rooms/id
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public ActionResult<RoomDTO> UpdateRoom(int id, UpdateRoomDTO updateRoomDto)
        {
            var room = _roomService.UpdateRoom(id, updateRoomDto);

            if (room == null)
            {
                return NotFound($"Комната с ID {id} не найдена");
            }

            return Ok(room);
        }

        // DELETE api/rooms/id
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteRoom(int id)
        {
            var result = _roomService.DeleteRoom(id);

            if (!result)
            {
                return NotFound($"Комната с ID {id} не найдена");
            }

            return NoContent();
        }
    }
}