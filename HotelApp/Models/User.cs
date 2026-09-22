using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelApp.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Login { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        public Role Role { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? Phone { get; set; }

        [MaxLength(200)]
        public string? PassportData { get; set; }

        // Навигационное свойство
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }

    public enum Role
    {
        Client = 0,
        Manager = 1,
        Admin = 2
    }
}