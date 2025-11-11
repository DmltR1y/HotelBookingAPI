namespace HotelBookingAPI.Models
{
    public class Room
    {
        public int RoomId { get; set; }
        public string RoomNumber { get; set; } = string.Empty;
        public int RoomTypeId { get; set; }
        public RoomType? RoomType { get; set; } 
        public decimal PricePerNight { get; set; }
        public int Capacity { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool IsAvailable { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
