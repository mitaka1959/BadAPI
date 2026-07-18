using System.ComponentModel.DataAnnotations;

namespace BadAPI.DTOs
{
    public class ReviewUpdateDto
    {
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; }

        [Required, StringLength(2000, MinimumLength = 1)]
        public string Comment { get; set; } = string.Empty;
    }
}
