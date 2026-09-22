using Microsoft.EntityFrameworkCore;
using HotelApp.Models;

namespace HotelApp.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<RoomType> RoomTypes { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BookingService> BookingServices { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Замени "твой_пароль" на реальный пароль от postgres
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=hotel_db;Username=postgres;Password=123");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Уникальность логина
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Login)
                .IsUnique();

            // Уникальность номера комнаты
            modelBuilder.Entity<Room>()
                .HasIndex(r => r.Number)
                .IsUnique();

            // Связь User -> Booking (один ко многим)
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.User)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Связь Room -> Booking (один ко многим)
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Room)
                .WithMany(r => r.Bookings)
                .HasForeignKey(b => b.RoomId)
                .OnDelete(DeleteBehavior.Restrict);

            // Связь RoomType -> Room (один ко многим)
            modelBuilder.Entity<Room>()
                .HasOne(r => r.RoomType)
                .WithMany(rt => rt.Rooms)
                .HasForeignKey(r => r.RoomTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Связь Booking -> BookingService (один ко многим)
            modelBuilder.Entity<BookingService>()
                .HasOne(bs => bs.Booking)
                .WithMany(b => b.BookingServices)
                .HasForeignKey(bs => bs.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            // Связь Service -> BookingService (один ко многим)
            modelBuilder.Entity<BookingService>()
                .HasOne(bs => bs.Service)
                .WithMany(s => s.BookingServices)
                .HasForeignKey(bs => bs.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}