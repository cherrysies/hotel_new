using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelApp.Models
{
    public class Room
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(10)]
        public string Number { get; set; } = string.Empty;

        [Required]
        public int RoomTypeId { get; set; }

        [ForeignKey("RoomTypeId")]
        public RoomType? RoomType { get; set; }

        [Required]
        public int Floor { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal PricePerDay { get; set; }

        [Required]
        public RoomStatus Status { get; set; } = RoomStatus.Available;

        [MaxLength(200)]
        public string? Description { get; set; }

        // Навигационное свойство
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }

    public enum RoomStatus
    {
        Available = 0,      // Свободен
        Occupied = 1,       // Занят
        Cleaning = 2        // Уборка
    }
}