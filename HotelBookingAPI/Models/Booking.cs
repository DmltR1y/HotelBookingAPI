namespace HotelBookingAPI.Models
{
    public class Booking
    {
        public int BookingId { get; set; }

        public int GuestId { get; set; }
        public int RoomId { get; set; }

        public Guest? Guest { get; set; }
        public Room? Room { get; set; }

        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int NumberOfGuests { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = "Pending"; // Pending, Confirmed, CheckedIn, CheckedOut, Cancelled
        public DateTime BookingDate { get; set; } = DateTime.UtcNow;
        public string? SpecialRequests { get; set; }
        public DateTime? CheckedInAt { get; set; }
        public DateTime? CheckedOutAt { get; set; }
    }
}
