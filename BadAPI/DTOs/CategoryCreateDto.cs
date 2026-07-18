using System.ComponentModel.DataAnnotations;

namespace BadAPI.DTOs
{
    public class CategoryCreateDto
    {
        [Required]
        [StringLength(200, MinimumLength = 1)]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;
    }
}
