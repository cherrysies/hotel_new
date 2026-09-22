using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelApp.Models
{
    public class Booking
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }

        [Required]
        public int RoomId { get; set; }

        [ForeignKey("RoomId")]
        public Room? Room { get; set; }

        [Required]
        public DateTime CheckIn { get; set; }

        [Required]
        public DateTime CheckOut { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalAmount { get; set; }

        [Required]
        public BookingStatus Status { get; set; } = BookingStatus.Pending;

        [Column(TypeName = "timestamp without time zone")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Навигационное свойство
        public ICollection<BookingService> BookingServices { get; set; } = new List<BookingService>();
    }

    public enum BookingStatus
    {
        Pending = 0,        // Ожидает подтверждения
        Confirmed = 1,      // Подтверждено
        CheckedIn = 2,      // Заселён
        CheckedOut = 3,     // Выписан
        Cancelled = 4       // Отменено
    }
}