using HotelApp.Data;
using HotelApp.Models;

namespace HotelApp.Services
{
    public static class DatabaseSeeder
    {
        public static void Seed(AppDbContext context)
        {
            // Проверяем, есть ли уже данные
            if (context.RoomTypes.Any())
                return;

            // Типы номеров
            var standardType = new RoomType
            {
                Name = "Стандарт",
                Description = "Одноместный номер с базовыми удобствами",
                BasePrice = 3000
            };

            var luxuryType = new RoomType
            {
                Name = "Люкс",
                Description = "Двухместный номер повышенной комфортности",
                BasePrice = 7000
            };

            var familyType = new RoomType
            {
                Name = "Семейный",
                Description = "Просторный номер для семьи с детьми",
                BasePrice = 9000
            };

            context.RoomTypes.AddRange(standardType, luxuryType, familyType);
            context.SaveChanges();

            // Номера
            var rooms = new List<Room>
            {
                new Room { Number = "101", RoomTypeId = standardType.Id, Floor = 1, PricePerDay = 3000, Status = RoomStatus.Available, Description = "Вид на двор" },
                new Room { Number = "102", RoomTypeId = standardType.Id, Floor = 1, PricePerDay = 3200, Status = RoomStatus.Available, Description = "Вид на улицу" },
                new Room { Number = "103", RoomTypeId = standardType.Id, Floor = 1, PricePerDay = 3100, Status = RoomStatus.Cleaning },
                new Room { Number = "201", RoomTypeId = luxuryType.Id, Floor = 2, PricePerDay = 7000, Status = RoomStatus.Available, Description = "Балкон, джакузи" },
                new Room { Number = "202", RoomTypeId = luxuryType.Id, Floor = 2, PricePerDay = 7500, Status = RoomStatus.Occupied },
                new Room { Number = "203", RoomTypeId = luxuryType.Id, Floor = 2, PricePerDay = 7200, Status = RoomStatus.Available, Description = "Панорамный вид" },
                new Room { Number = "301", RoomTypeId = familyType.Id, Floor = 3, PricePerDay = 9000, Status = RoomStatus.Available, Description = "2 спальни, кухня" },
                new Room { Number = "302", RoomTypeId = familyType.Id, Floor = 3, PricePerDay = 9500, Status = RoomStatus.Available, Description = "Детская комната" },
                new Room { Number = "303", RoomTypeId = familyType.Id, Floor = 3, PricePerDay = 9200, Status = RoomStatus.Cleaning },
                new Room { Number = "401", RoomTypeId = luxuryType.Id, Floor = 4, PricePerDay = 8000, Status = RoomStatus.Available, Description = "Президентский люкс" }
            };

            context.Rooms.AddRange(rooms);
            context.SaveChanges();

            // Услуги
            var services = new List<Service>
            {
                new Service { Name = "Завтрак", Description = "Континентальный завтрак в ресторане", Price = 500 },
                new Service { Name = "Трансфер из аэропорта", Description = "Встреча и доставка до отеля", Price = 1500 },
                new Service { Name = "Сауна", Description = "Час посещения сауны", Price = 1000 },
                new Service { Name = "Парковка", Description = "Суточная парковка на территории", Price = 300 },
                new Service { Name = "Прачечная", Description = "Стирка и глажка одежды", Price = 400 }
            };

            context.Services.AddRange(services);
            context.SaveChanges();

            // Пользователи (пароли хешируются через BCrypt)
            var admin = new User
            {
                Login = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                Role = Role.Admin,
                FullName = "Иванов Иван Иванович",
                Phone = "+7 (900) 123-45-67"
            };

            var manager = new User
            {
                Login = "manager",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("manager123"),
                Role = Role.Manager,
                FullName = "Петрова Мария Сергеевна",
                Phone = "+7 (900) 234-56-78"
            };

            var client1 = new User
            {
                Login = "client1",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("client123"),
                Role = Role.Client,
                FullName = "Сидоров Алексей Петрович",
                Phone = "+7 (900) 345-67-89",
                PassportData = "4510 123456, выдан ОВД Тверской г. Москвы"
            };

            var client2 = new User
            {
                Login = "client2",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("client123"),
                Role = Role.Client,
                FullName = "Козлова Елена Дмитриевна",
                Phone = "+7 (900) 456-78-90",
                PassportData = "4511 654321, выдан ОВД Арбат г. Москвы"
            };

            context.Users.AddRange(admin, manager, client1, client2);
            context.SaveChanges();

            // Бронирования
            var booking1 = new Booking
            {
                UserId = client1.Id,
                RoomId = rooms[3].Id, // номер 201
                CheckIn = DateTime.Today.AddDays(-2),
                CheckOut = DateTime.Today.AddDays(3),
                TotalAmount = 35000,
                Status = BookingStatus.CheckedIn,
                CreatedAt = DateTime.Now,  // Не Utc, не SpecifyKind
            };

            var booking2 = new Booking
            {
                UserId = client2.Id,
                RoomId = rooms[0].Id, // номер 101
                CheckIn = DateTime.Today.AddDays(1),
                CheckOut = DateTime.Today.AddDays(4),
                TotalAmount = 9600,
                Status = BookingStatus.Confirmed,
                CreatedAt = DateTime.Now,  // Не Utc, не SpecifyKind
            };

            context.Bookings.AddRange(booking1, booking2);
            context.SaveChanges();

            // Доп. услуги к бронированию
            var bookingServices = new List<BookingService>
            {
                new BookingService
                {
                    BookingId = booking1.Id,
                    ServiceId = services[0].Id, // Завтрак
                    Quantity = 5, // 5 дней
                    TotalPrice = 2500
                },
                new BookingService
                {
                    BookingId = booking1.Id,
                    ServiceId = services[1].Id, // Трансфер
                    Quantity = 1,
                    TotalPrice = 1500
                }
            };

            context.BookingServices.AddRange(bookingServices);
            context.SaveChanges();
        }
    }
}