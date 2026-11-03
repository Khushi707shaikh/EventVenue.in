
using EventVenue.Models;

namespace EventVenue.Services
{
    public interface IVenueService
    {
        Task<List<Venue>> GetAllVenuesAsync();

        Task<Venue?> GetVenueByIdAsync(int id);

        Task AddVenueAsync(Venue venue);

        Task UpdateVenueAsync(Venue venue);

        Task DeleteVenueAsync(int id);
    }
}
