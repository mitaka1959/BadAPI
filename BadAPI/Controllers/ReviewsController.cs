using BadAPI.Data.Entities;
using BadAPI.DTOs;
using BadAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BadAPI.Controllers
{
    [ApiController]
    public class ReviewsController : ApiControllerBase
    {
        private readonly ReviewService _service;

        public ReviewsController(ReviewService service)
        {
            _service = service;
        }

        [HttpPost("api/products/{productId:guid}/reviews")]
        public async Task<IActionResult> Create(Guid productId, [FromBody] ReviewCreateDto dto)
        {
            var result = await _service.AddReviewAsync(productId, dto.ReviewerName, dto.Rating, dto.Comment);
            if (!result.IsSuccess)
                return HandleFailure(result);

            var read = ToReadDto(result.Value!);
            return CreatedAtAction(nameof(GetById), new { id = read.Id }, read);
        }

        [HttpGet("api/products/{productId:guid}/reviews")]
        public async Task<IActionResult> GetForProduct(Guid productId)
        {
            var result = await _service.GetReviewsForProductAsync(productId);
            return result.IsSuccess
                ? Ok(result.Value!.Select(ToReadDto))
                : HandleFailure(result);
        }

        [HttpGet("api/reviews/{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetReviewAsync(id);
            return result.IsSuccess ? Ok(ToReadDto(result.Value!)) : HandleFailure(result);
        }

        [HttpPut("api/reviews/{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ReviewUpdateDto dto)
        {
            var result = await _service.UpdateReviewAsync(id, dto.Rating, dto.Comment);
            return result.IsSuccess ? Ok(ToReadDto(result.Value!)) : HandleFailure(result);
        }

        [HttpDelete("api/reviews/{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _service.DeleteReviewAsync(id);
            return result.IsSuccess ? NoContent() : HandleFailure(result);
        }

        private static ReviewReadDto ToReadDto(Review r) => new()
        {
            Id = r.Id,
            ProductId = r.ProductId,
            ReviewerName = r.ReviewerName,
            Rating = r.Rating,
            Comment = r.Comment,
            CreatedAtUtc = r.CreatedAtUtc,
            UpdatedAtUtc = r.UpdatedAtUtc
        };
    }
}
