using EventVenue.Data;
using EventVenue.Models;
using EventVenue.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventVenue.Controllers
{
    public class UserController(IVenueService venueService, ApplicationDbContext context) : Controller
    {
        private readonly IVenueService _venueService = venueService;
        private readonly ApplicationDbContext _context = context;

        public async Task<IActionResult> Index()
        {
            var venues = await _venueService.GetAllVenuesAsync();

            return View(venues);
        }

        public async Task<IActionResult> VenueDetails(int id)
        {
            var venue = await _venueService.GetVenueByIdAsync(id);

            if (venue == null)
            {
                return NotFound();
            }

            return View(venue);
        }

        [HttpGet]
        public async Task<IActionResult> BookVenue(int id)
        {
            // Check whether user is logged in
            if (HttpContext.Session.GetInt32("AppUserId") == null)
            {
                return RedirectToAction(nameof(Login), new
                {
                    returnUrl = Url.Action(
                        nameof(BookVenue),
                        "User",
                        new { id = id })
                });
            }

            var venue = await _venueService.GetVenueByIdAsync(id);

            if (venue == null || !venue.IsAvailable)
            {
                return NotFound();
            }

            ViewBag.Venue = venue;

            return View(new Booking
            {
                VenueId = venue.VenueId,
                EventDate = DateTime.Today.AddDays(1),
                GuestCount = 1
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BookVenue(Booking booking)
        {
            // Get the currently logged-in user's ID from Session
            int? userId = HttpContext.Session.GetInt32("AppUserId");

            if (userId == null)
            {
                return RedirectToAction(nameof(Login), new
                {
                    returnUrl = Url.Action(
                        nameof(BookVenue),
                        "User",
                        new { id = booking.VenueId })
                });
            }

            // Find the selected venue
            var venue = await _venueService.GetVenueByIdAsync(booking.VenueId);

            if (venue == null || !venue.IsAvailable)
            {
                return NotFound();
            }

            // Validate event date
            if (booking.EventDate.Date < DateTime.Today)
            {
                ModelState.AddModelError(
                    nameof(booking.EventDate),
                    "Event date cannot be in the past.");
            }

            // Validate guest count
            if (booking.GuestCount < 1 ||
                booking.GuestCount > venue.Capacity)
            {
                ModelState.AddModelError(
                    nameof(booking.GuestCount),
                    $"Guest count must be between 1 and {venue.Capacity}.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Venue = venue;
                return View(booking);
            }

            // Save booking for the logged-in user
            booking.AppUserId = userId.Value;
            booking.Status = "Pending";
            booking.BookingCreatedAt = DateTime.Now;

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return RedirectToAction(
                nameof(BookingConfirmation),
                new { id = booking.BookingId });
        }

        public async Task<IActionResult> BookingConfirmation(int id)
        {
            int? userId = HttpContext.Session.GetInt32("AppUserId");

            if (userId == null)
            {
                return RedirectToAction(nameof(Login));
            }

            var booking = await _context.Bookings
                .Include(b => b.Venue)
                .FirstOrDefaultAsync(b =>
                    b.BookingId == id &&
                    b.AppUserId == userId.Value);

            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        [HttpGet]
        public async Task<IActionResult> MyBookings()
        {
            int? userId = HttpContext.Session.GetInt32("AppUserId");

            if (userId == null)
            {
                return RedirectToAction(nameof(Login));
            }

            var bookings = await _context.Bookings
                .Include(b => b.Venue)
                .Where(b => b.AppUserId == userId.Value)
                .OrderByDescending(b => b.BookingId)
                .ToListAsync();

            return View(bookings);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelBooking(int id)
        {
            int? userId = HttpContext.Session.GetInt32("AppUserId");

            if (userId == null)
            {
                return RedirectToAction(nameof(Login));
            }

            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b =>
                    b.BookingId == id &&
                    b.AppUserId == userId.Value);

            if (booking == null)
            {
                return NotFound();
            }

            if (booking.Status == "Pending")
            {
                booking.Status = "Cancelled";
                await _context.SaveChangesAsync();

                TempData["Success"] = "Your booking has been cancelled.";
            }

            return RedirectToAction(nameof(MyBookings));
        }

        // GET: /User/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /User/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(AppUser user)
        {
            if (!ModelState.IsValid)
            {
                return View(user);
            }

            bool emailExists = await _context.AppUsers
                .AnyAsync(u => u.Email == user.Email);

            if (emailExists)
            {
                ModelState.AddModelError(
                    nameof(user.Email),
                    "This email is already registered.");

                return View(user);
            }

            _context.AppUsers.Add(user);
            await _context.SaveChangesAsync();

            TempData["Success"] =
                "Registration successful! Please log in.";

            return RedirectToAction(nameof(Login));
        }

        // GET: /User/Login
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: /User/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(
    string email,
    string password,
    string? returnUrl)
        {
            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Please enter your email and password.";
                ViewBag.ReturnUrl = returnUrl;
                return View();
            }

            var user = await _context.AppUsers.FirstOrDefaultAsync(u =>
                u.Email == email && u.Password == password);

            if (user == null)
            {
                ViewBag.Error = "Invalid email or password.";
                ViewBag.ReturnUrl = returnUrl;
                return View();
            }

            HttpContext.Session.SetInt32("AppUserId", user.AppUserId);
            HttpContext.Session.SetString("AppUserName", user.FullName);

            // Return user to the requested local page
            if (!string.IsNullOrEmpty(returnUrl) &&
                Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: /User/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction(nameof(Login));
        }
    }

}

