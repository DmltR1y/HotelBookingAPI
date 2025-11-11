namespace HotelBookingAPI.DTOs
{
    public class RoomDTO
    {
        public int RoomId { get; set; }
        public string RoomNumber { get; set; } = string.Empty;
        public string RoomTypeName { get; set; } = string.Empty;
        public decimal PricePerNight { get; set; }
        public int Capacity { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool IsAvailable { get; set; }
    }

    public class CreateRoomDTO
    {
        public string RoomNumber { get; set; } = string.Empty;
        public int RoomTypeId { get; set; }
        public decimal PricePerNight { get; set; }
        public int Capacity { get; set; }
        public string Description { get; set; } = string.Empty;
    }

    public class UpdateRoomDTO
    {
        public string RoomNumber { get; set; } = string.Empty;
        public int RoomTypeId { get; set; }
        public decimal PricePerNight { get; set; }
        public int Capacity { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool IsAvailable { get; set; }
    }

    public class RoomAvailabilityRequestDTO
    {
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
    }
}
