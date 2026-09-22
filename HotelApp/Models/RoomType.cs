using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelApp.Models
{
    public class RoomType
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Description { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal BasePrice { get; set; }

        // Навигационное свойство
        public ICollection<Room> Rooms { get; set; } = new List<Room>();
    }
}