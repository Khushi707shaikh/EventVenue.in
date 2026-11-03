using EventVenue.Data;
using EventVenue.Services;
using EventVenue.Models;
using Microsoft.EntityFrameworkCore;

namespace EventVenue.Services
{
    public class BookingService : IBookingService
    {
        private readonly ApplicationDbContext _context;

        public BookingService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<Booking>> GetAllAsync()
        {
            return await _context.Bookings
                .Include(b => b.Venue)
                .Include(b => b.AppUser)
                .OrderByDescending(b => b.BookingId)
                .ToListAsync();
        }

        public async Task<List<Booking>> GetUserBookingsAsync(int appUserId)
        {
            return await _context.Bookings
                .Include(b => b.Venue)
                .Where(b => b.AppUserId == appUserId)
                .OrderByDescending(b => b.BookingId)
                .ToListAsync();
        }

        public async Task<Booking?> GetByIdAsync(int id)
        {
            return await _context.Bookings
                .Include(b => b.Venue)
                .Include(b => b.AppUser)
                .FirstOrDefaultAsync(b => b.BookingId == id);
        }

        public async Task<bool> CreateAsync(Booking booking)
        {
            _context.Bookings.Add(booking);

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task UpdateStatusAsync(int id, string status)
        {
            var booking = await _context.Bookings.FindAsync(id);

            if (booking != null)
            {
                booking.Status = status;
                await _context.SaveChangesAsync();
            }
        }

        public async Task CancelAsync(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);

            if (booking != null)
            {
                booking.Status = "Cancelled";
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);

            if (booking != null)
            {
                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();
            }
        }
    }
}