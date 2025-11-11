namespace HotelBookingAPI.DTOs
{
    public class BookingDTO
    {
        public int BookingId { get; set; }
        public int GuestId { get; set; }
        public string GuestName { get; set; } = string.Empty;
        public int RoomId { get; set; }
        public string RoomNumber { get; set; } = string.Empty;
        public string RoomType { get; set; } = string.Empty;
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int NumberOfGuests { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime BookingDate { get; set; }
        public string? SpecialRequests { get; set; }
        public int Nights => (CheckOutDate - CheckInDate).Days;
    }

    public class CreateBookingDTO
    {
        public int GuestId { get; set; }
        public int RoomId { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int NumberOfGuests { get; set; }
        public string? SpecialRequests { get; set; }
    }

    public class UpdateBookingStatusDTO
    {
        public string Status { get; set; } = string.Empty;
    }

    public class BookingResponseDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public BookingDTO? Booking { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }
}
