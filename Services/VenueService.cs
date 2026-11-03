
using EventVenue.Data;
using EventVenue.Models;
using Microsoft.EntityFrameworkCore;

namespace EventVenue.Services
{
    public class VenueService(ApplicationDbContext context) : IVenueService
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<List<Venue>> GetAllVenuesAsync()
        {
            return await _context.Venues
                .AsNoTracking()
                .OrderBy(v => v.VenueName)
                .ToListAsync();
        }

        public async Task<Venue?> GetVenueByIdAsync(int id)
        {
            return await _context.Venues
                .FirstOrDefaultAsync(v => v.VenueId == id);
        }

        public async Task AddVenueAsync(Venue venue)
        {
            _context.Venues.Add(venue);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateVenueAsync(Venue venue)
        {
            _context.Venues.Update(venue);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteVenueAsync(int id)
        {
            var venue = await _context.Venues.FindAsync(id);

            if (venue != null)
            {
                _context.Venues.Remove(venue);
                await _context.SaveChangesAsync();
            }
        }
    }
}
