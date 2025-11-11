using AutoMapper;
using HotelBookingAPI.DTOs;
using HotelBookingAPI.Interfaces;
using HotelBookingAPI.Models;

namespace HotelBookingAPI.Services
{
    public class RoomService : IRoomService
    {
        private IRoomRepository _roomRepository;
        private IMapper _mapper;

        public RoomService(IRoomRepository roomRepository, IMapper mapper)
        {
            _roomRepository = roomRepository;
            _mapper = mapper;
        }

        public IEnumerable<RoomDTO> GetAllRooms()
        {
            var rooms = _roomRepository.GetAllRooms();
            return _mapper.Map<IEnumerable<RoomDTO>>(rooms);
        }

        public RoomDTO? GetRoomById(int id)
        {
            var room = _roomRepository.GetRoomById(id);
            return _mapper.Map<RoomDTO>(room);
        }

        public RoomDTO CreateRoom(CreateRoomDTO createRoomDto)
        {
            if (_roomRepository.RoomNumberExists(createRoomDto.RoomNumber))
            {
                throw new ArgumentException($"Номер {createRoomDto.RoomNumber} уже существует");
            }

            var room = _mapper.Map<Room>(createRoomDto);
            var createdRoom = _roomRepository.AddRoom(room);
            return _mapper.Map<RoomDTO>(createdRoom);
        }

        public RoomDTO? UpdateRoom(int id, UpdateRoomDTO updateRoomDto)
        {
            var room = _mapper.Map<Room>(updateRoomDto);
            room.RoomId = id;

            var updatedRoom =_roomRepository.UpdateRoom(id, room);
            return _mapper.Map<RoomDTO>(updatedRoom);
        }

        public bool DeleteRoom(int id)
        {
            var room = _roomRepository.DeleteRoom(id);
            return room != null;
        }

        public IEnumerable<RoomDTO> GetAvailableRooms(RoomAvailabilityRequestDTO request)
        {
            if (request.CheckInDate >= request.CheckOutDate)
            {
                throw new ArgumentException("Дата выезда должна быть позже даты заезда");
            }

            if (request.CheckInDate < DateTime.Today)
            {
                throw new ArgumentException("Дата заезда не может быть в прошлом");
            }

            var rooms = _roomRepository.GetAvailableRooms(request.CheckInDate, request.CheckOutDate);
            return _mapper.Map<IEnumerable<RoomDTO>>(rooms);
        }
    }
}
