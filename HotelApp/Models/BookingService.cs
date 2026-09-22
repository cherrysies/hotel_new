using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelApp.Models
{
    public class BookingService
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int BookingId { get; set; }

        [ForeignKey("BookingId")]
        public Booking? Booking { get; set; }

        [Required]
        public int ServiceId { get; set; }

        [ForeignKey("ServiceId")]
        public Service? Service { get; set; }

        [Required]
        public int Quantity { get; set; } = 1;

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalPrice { get; set; }
    }
}