
using System.ComponentModel.DataAnnotations;

namespace EventVenue.Models
{
    public class Venue
    {
        [Key]
        public int VenueId { get; set; }

        [Required]
        [StringLength(100)]
        public string VenueName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string VenueType { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Location { get; set; } = string.Empty;

        [Range(1, 100000)]
        public int Capacity { get; set; }

        [Range(0, 100000000)]
        public decimal Price { get; set; }

        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }

        public bool IsAvailable { get; set; } = true;
    }
}
