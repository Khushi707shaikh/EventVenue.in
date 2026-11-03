
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventVenue.Models
{
    public class Booking
    {
        [Key]
        public int BookingId { get; set; }

        [Required]
        public int AppUserId { get; set; }

        [ForeignKey(nameof(AppUserId))]
        public AppUser? AppUser { get; set; }

        [Required]
        public int VenueId { get; set; }

        [ForeignKey(nameof(VenueId))]
        public Venue? Venue { get; set; }

        [Required]
        [StringLength(100)]
        public string EventType { get; set; } = string.Empty;

        [Required]
        public DateTime EventDate { get; set; }

        [Range(1, 100000)]
        public int GuestCount { get; set; }

        [StringLength(30)]
        public string Status { get; set; } = "Pending";

        public DateTime BookingCreatedAt { get; set; } = DateTime.Now;
    }
}
