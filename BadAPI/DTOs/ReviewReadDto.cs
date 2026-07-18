namespace BadAPI.DTOs
{
    public class ReviewReadDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string ReviewerName { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
    }
}
