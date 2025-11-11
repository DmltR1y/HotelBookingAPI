using AutoMapper;
using HotelBookingAPI.DTOs;
using HotelBookingAPI.Models;

namespace HotelBookingAPI
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Room
            CreateMap<Room, RoomDTO>()
                .ForMember(dest => dest.RoomTypeName, opt => opt.MapFrom(src => src.RoomType.Name));
            CreateMap<CreateRoomDTO, Room>();
            CreateMap<UpdateRoomDTO, Room>();

            // RoomType
            CreateMap<RoomType, RoomTypeDTO>();
            CreateMap<CreateRoomTypeDTO, RoomType>();
            CreateMap<UpdateRoomTypeDTO, RoomType>();

            // Guest
            CreateMap<Guest, GuestDTO>();
            CreateMap<CreateGuestDTO, Guest>();
            CreateMap<UpdateGuestDTO, Guest>();

            // Booking
            CreateMap<Booking, BookingDTO>()
                .ForMember(dest => dest.GuestName, opt => opt.MapFrom(src => $"{src.Guest!.FirstName} {src.Guest.LastName}"))
                .ForMember(dest => dest.RoomNumber, opt => opt.MapFrom(src => src.Room!.RoomNumber))
                .ForMember(dest => dest.RoomType, opt => opt.MapFrom(src => src.Room!.RoomType!.Name));
            CreateMap<CreateBookingDTO, Booking>();
        }
    }
}
