using EventVenue.Models;
namespace EventVenue.Services 
{ 

public interface IBookingService
    {
        Task<List<Booking>> GetAllAsync();

        Task<List<Booking>> GetUserBookingsAsync(int appUserId);

        Task<Booking?> GetByIdAsync(int id);

        Task<bool> CreateAsync(Booking booking);

        Task UpdateStatusAsync(int id, string status);

        Task CancelAsync(int id);

        Task DeleteAsync(int id);

    }
}