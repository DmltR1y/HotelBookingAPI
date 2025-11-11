using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using HotelBookingAPI.Data;
using HotelBookingAPI.Models;
using HotelBookingAPI.DTOs;
using AutoMapper;

namespace HotelBookingAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RoomTypesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public RoomTypesController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/roomtypes
        [HttpGet]
        [AllowAnonymous]
        public ActionResult<IEnumerable<RoomTypeDTO>> GetRoomTypes()
        {
            var roomTypes = _context.RoomTypes.ToList();
            return Ok(_mapper.Map<IEnumerable<RoomTypeDTO>>(roomTypes));
        }

        // GET api/roomtypes/id
        [HttpGet("{id}")]
        [AllowAnonymous]
        public ActionResult<RoomTypeDTO> GetRoomType(int id)
        {
            var roomType = _context.RoomTypes.Find(id);

            if (roomType == null)
            {
                return NotFound($"Тип комнаты с ID {id} не найден");
            }

            return Ok(_mapper.Map<RoomTypeDTO>(roomType));
        }

        // POST api/roomtypes
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public ActionResult<RoomTypeDTO> CreateRoomType(CreateRoomTypeDTO createRoomTypeDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var roomType = _mapper.Map<RoomType>(createRoomTypeDto);

            _context.RoomTypes.Add(roomType);
            _context.SaveChanges();

            var roomTypeDto = _mapper.Map<RoomTypeDTO>(roomType);
            return CreatedAtAction(nameof(GetRoomType), new { id = roomType.RoomTypeId }, roomTypeDto);
        }

        // PUT api/roomtypes/id
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public ActionResult<RoomTypeDTO> UpdateRoomType(int id, UpdateRoomTypeDTO updateRoomTypeDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingRoomType = _context.RoomTypes.Find(id);
            if (existingRoomType == null)
            {
                return NotFound($"Тип комнаты с ID {id} не найден");
            }

            // Обновляем поля
            existingRoomType.Name = updateRoomTypeDto.Name;
            existingRoomType.Description = updateRoomTypeDto.Description;

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoomTypeExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return Ok(_mapper.Map<RoomTypeDTO>(existingRoomType));
        }

        // DELETE api/roomtypes/id
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteRoomType(int id)
        {
            var roomType = _context.RoomTypes.Find(id);
            if (roomType == null)
            {
                return NotFound($"Тип комнаты с ID {id} не найден");
            }

            // Проверяем нет ли комнат с этим типом
            var hasRooms = _context.Rooms.Any(r => r.RoomTypeId == id);
            if (hasRooms)
            {
                return BadRequest("Нельзя удалить тип номера, так как есть комнаты с этим типом");
            }

            _context.RoomTypes.Remove(roomType);
            _context.SaveChanges();

            return NoContent();
        }

        private bool RoomTypeExists(int id)
        {
            return _context.RoomTypes.Any(e => e.RoomTypeId == id);
        }
    }
}