using EventVenue.Data;
using EventVenue.Models;
using EventVenue.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventVenue.Controllers
{
    public class AdminController(
    IVenueService venueService,
    IBookingService bookingService,
    ApplicationDbContext context) : Controller
    {
        private readonly IVenueService _venueService = venueService;
        private readonly IBookingService _bookingService = bookingService;
        private readonly ApplicationDbContext _context = context;
        // Admin dashboard
        public IActionResult Index()
        {
            return View();
        }

        // Display all venues
        public async Task<IActionResult> Venues()
        {
            var venues = await _venueService.GetAllVenuesAsync();
            return View(venues);
        }

        // Display add venue form
        [HttpGet]
        public IActionResult CreateVenue()
        {
            return View();
        }

        // Save new venue
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateVenue(Venue venue)
        {
            if (!ModelState.IsValid)
                return View(venue);

            await _venueService.AddVenueAsync(venue);

            return RedirectToAction(nameof(Venues));
        }

        // Display edit venue form
        [HttpGet]
        public async Task<IActionResult> EditVenue(int id)
        {
            var venue = await _venueService.GetVenueByIdAsync(id);

            if (venue == null)
                return NotFound();

            return View(venue);
        }

        // Save venue updates
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditVenue(int id, Venue venue)
        {
            if (id != venue.VenueId)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(venue);

            var existingVenue = await _venueService.GetVenueByIdAsync(id);

            if (existingVenue == null)
                return NotFound();

            existingVenue.VenueName = venue.VenueName;
            existingVenue.VenueType = venue.VenueType;
            existingVenue.Location = venue.Location;
            existingVenue.Capacity = venue.Capacity;
            existingVenue.Price = venue.Price;
            existingVenue.Description = venue.Description;
            existingVenue.ImageUrl = venue.ImageUrl;
            existingVenue.IsAvailable = venue.IsAvailable;

            await _venueService.UpdateVenueAsync(existingVenue);

            return RedirectToAction(nameof(Venues));
        }

        // Delete venue
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteVenue(int id)
        {
            await _venueService.DeleteVenueAsync(id);
            return RedirectToAction(nameof(Venues));
        }
         // GET: /Admin/Login
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    // POST: /Admin/Login
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) ||
            string.IsNullOrWhiteSpace(password))
        {
            ViewBag.Error = "Please enter username and password.";
            return View();
        }

        var admin = await _context.Admins.FirstOrDefaultAsync(a =>
            a.Username == username && a.Password == password);

        if (admin == null)
        {
            ViewBag.Error = "Invalid username or password.";
            return View();
        }

        HttpContext.Session.SetInt32("AdminId", admin.AdminId);
        HttpContext.Session.SetString("AdminUsername", admin.Username);

        return RedirectToAction(nameof(Index));
    }

    // GET: /Admin/Logout
    public IActionResult Logout()
    {
        HttpContext.Session.Remove("AdminId");
        HttpContext.Session.Remove("AdminUsername");

        return RedirectToAction(nameof(Login));
    }
        public async Task<IActionResult> Bookings()
        {
            if (HttpContext.Session.GetInt32("AdminId") == null)
                return RedirectToAction("Login");

            var bookings = await _bookingService.GetAllAsync();

            return View(bookings);
        }
        public async Task<IActionResult> BookingDetails(int id)
        {
            if (HttpContext.Session.GetInt32("AdminId") == null)
                return RedirectToAction("Login");

            var booking = await _bookingService.GetByIdAsync(id);

            if (booking == null)
                return NotFound();

            return View(booking);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateBookingStatus(
    int id, string status)
        {
            if (HttpContext.Session.GetInt32("AdminId") == null)
                return RedirectToAction("Login");

            await _bookingService.UpdateStatusAsync(id, status);

            TempData["Success"] = "Booking status updated successfully!";

            return RedirectToAction("Bookings");
        }
    }
}
