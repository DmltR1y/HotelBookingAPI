using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using HotelBookingAPI.Models;

namespace HotelBookingAPI.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Room> Rooms { get; set; }
        public DbSet<RoomType> RoomTypes { get; set; }
        public DbSet<Guest> Guests { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Booking -> Guest 
            builder.Entity<Booking>()
                .HasOne(b => b.Guest)
                .WithMany(g => g.Bookings)
                .HasForeignKey(b => b.GuestId)
                .OnDelete(DeleteBehavior.Restrict);

            // Booking -> Room
            builder.Entity<Booking>()
                .HasOne(b => b.Room)
                .WithMany(r => r.Bookings)
                .HasForeignKey(b => b.RoomId)
                .OnDelete(DeleteBehavior.Restrict);

            // Room -> RoomType
            builder.Entity<Room>()
                .HasOne(r => r.RoomType)
                .WithMany(rt => rt.Rooms)
                .HasForeignKey(r => r.RoomTypeId);

            builder.Entity<RoomType>().HasData(
                new RoomType { RoomTypeId = 1, Name = "Стандартный", Description = "Стандартный номер с базовыми удобствами" },
                new RoomType { RoomTypeId = 2, Name = "Полулюкс", Description = "Улучшенный номер с дополнительным пространством" },
                new RoomType { RoomTypeId = 3, Name = "Люкс", Description = "Люкс номер с гостиной зоной" },
                new RoomType { RoomTypeId = 4, Name = "Семейный", Description = "Семейный номер" }
            );

            builder.Entity<Room>().HasData(
                new Room { RoomId = 1, RoomNumber = "101", RoomTypeId = 1, PricePerNight = 100.00m, Capacity = 2, Description = "Стандартный номер с одной двуспальной кроватью" },
                new Room { RoomId = 2, RoomNumber = "102", RoomTypeId = 1, PricePerNight = 100.00m, Capacity = 2, Description = "Стандартный номер с двумя односпальными кроватями" },
                new Room { RoomId = 3, RoomNumber = "201", RoomTypeId = 2, PricePerNight = 150.00m, Capacity = 3, Description = "Улучшенный номер с видом на город" },
                new Room { RoomId = 4, RoomNumber = "202", RoomTypeId = 2, PricePerNight = 150.00m, Capacity = 2, Description = "Улучшенный номер с балконом" },
                new Room { RoomId = 5, RoomNumber = "301", RoomTypeId = 3, PricePerNight = 250.00m, Capacity = 4, Description = "Люкс номер с гостиной и кухней" },
                new Room { RoomId = 6, RoomNumber = "302", RoomTypeId = 4, PricePerNight = 180.00m, Capacity = 5, Description = "Семейный номер с двумя спальнями" }
            );

            builder.Entity<Guest>().HasData(
                new Guest
                {
                    GuestId = 1,
                    FirstName = "Иван",
                    LastName = "Петров",
                    Email = "ivan.petrov@mail.com",
                    Phone = "+7-123-456-7890",
                    PassportNumber = "675869878",
                    DateOfBirth = new DateTime(1985, 5, 15),
                    Address = "Москва, ул. Примерная, д. 1"
                },
                new Guest
                {
                    GuestId = 2,
                    FirstName = "Мария",
                    LastName = "Сидорова",
                    Email = "maria.sidorova@mail.com",
                    Phone = "+7-987-654-3210",
                    PassportNumber = "74576587789",
                    DateOfBirth = new DateTime(1990, 8, 22),
                    Address = "Санкт-Петербург, Невский пр., д. 100"
                }
            );

            SeedIdentityData(builder);
        }

        private void SeedIdentityData(ModelBuilder builder)
        {
            // Создаем роли
            var adminRole = new IdentityRole
            {
                Id = "1",
                Name = "Admin",
                NormalizedName = "ADMIN",
                ConcurrencyStamp = Guid.NewGuid().ToString()
            };

            var managerRole = new IdentityRole
            {
                Id = "2",
                Name = "Manager",
                NormalizedName = "MANAGER",
                ConcurrencyStamp = Guid.NewGuid().ToString()
            };

            var receptionistRole = new IdentityRole
            {
                Id = "3",
                Name = "Receptionist",
                NormalizedName = "RECEPTIONIST",
                ConcurrencyStamp = Guid.NewGuid().ToString()
            };

            builder.Entity<IdentityRole>().HasData(adminRole, managerRole, receptionistRole);

            // Создаем администратора
            var adminUser = new ApplicationUser
            {
                Id = "1",
                UserName = "admin@hotel.com",
                NormalizedUserName = "ADMIN@HOTEL.COM",
                Email = "admin@hotel.com",
                NormalizedEmail = "ADMIN@HOTEL.COM",
                EmailConfirmed = true,
                FirstName = "Системный",
                LastName = "Администратор",
                Position = "Admin",
                HireDate = DateTime.UtcNow,
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                LockoutEnabled = true
            };

            var passwordHasher = new PasswordHasher<ApplicationUser>();
            adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, "Admin123!");

            builder.Entity<ApplicationUser>().HasData(adminUser);

            // Назначаем роли администратору
            builder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>
                {
                    RoleId = adminRole.Id,
                    UserId = adminUser.Id
                },
                new IdentityUserRole<string>
                {
                    RoleId = managerRole.Id,
                    UserId = adminUser.Id
                },
                new IdentityUserRole<string>
                {
                    RoleId = receptionistRole.Id,
                    UserId = adminUser.Id
                }
            );
        }
    }
}