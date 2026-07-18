using System.ComponentModel.DataAnnotations;

namespace BadAPI.DTOs
{
    public class ProductCreateDto
    {
        [Required]
        [StringLength(200, MinimumLength = 1)]
        public string Name { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
        public decimal Price { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 1)]
        public string CategoryName { get; set; } = string.Empty;
    }
}
