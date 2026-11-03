
using EventVenue.Models;
using Microsoft.EntityFrameworkCore;

namespace EventVenue.Data
{
    public class ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : DbContext(options)
    {
        public DbSet<Venue> Venues { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Admin> Admins { get; set; }
    }
}
