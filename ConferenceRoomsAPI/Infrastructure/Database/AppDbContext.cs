using ConferenceRoomsAPI.Domain.Entities;
using ConferenceRoomsAPI.Domain.Services;
using Microsoft.EntityFrameworkCore;

namespace ConferenceRoomsAPI.Infrastructure.Database
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Room> Rooms => Set<Room>();
        public DbSet<Booking> Bookings => Set<Booking>();
        public DbSet<Service> Services => Set<Service>();
        public DbSet<RoomService> RoomServices => Set<RoomService>();
        public DbSet<BookingService> BookingServices => Set<BookingService>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // RoomService - composite PK (RoomId + ServiceId)

            modelBuilder.Entity<RoomService>()
                .HasKey(rs => new { rs.RoomId, rs.ServiceId });

            modelBuilder.Entity<RoomService>()
                .HasOne(rs => rs.Room)
                .WithMany(r => r.RoomServices)
                .HasForeignKey(rs => rs.RoomId);

            modelBuilder.Entity<RoomService>()
                .HasOne(rs => rs.Service)
                .WithMany(s => s.RoomServices)
                .HasForeignKey(rs => rs.ServiceId);


            // BookingService - composite PK (BookingId + ServiceId)

            modelBuilder.Entity<BookingService>()
               .HasKey(bs => new { bs.BookingId, bs.ServiceId });

            modelBuilder.Entity<BookingService>()
                .HasOne(bs => bs.Booking)
                .WithMany(b => b.BookingServices)
                .HasForeignKey(bs => bs.BookingId);

            modelBuilder.Entity<BookingService>()
                .HasOne(bs => bs.Service)
                .WithMany(s => s.BookingServices)
                .HasForeignKey(bs => bs.ServiceId);


            // Room - column constrains

            modelBuilder.Entity<Room>(entity =>
            {
                entity.Property(r => r.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(r => r.HourlyPrice)
                    .HasPrecision(18,2);

                entity.HasQueryFilter(r => !r.IsDeleted);

            });


            // Service - column contrains

            modelBuilder.Entity<Service>(entity =>
            {
                entity.Property(s => s.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(s => s.Price)
                    .HasPrecision(18, 2);

                entity.HasQueryFilter(s => !s.IsDeleted);

            });


            // Booking - column contrains

            modelBuilder.Entity<Booking>(entity =>
            {
                entity.Property(b => b.TotalPrice)
                    .HasPrecision(18, 2);

                entity.Property(b => b.Status)
                    .HasConversion<string>();

            });


            // BookingService - price snapshot

            modelBuilder.Entity<BookingService>()
                .Property(bs => bs.PriceAtBooking)
                .HasPrecision(18, 2);
        }

    }
}
