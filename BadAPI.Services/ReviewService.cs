using BadAPI.Data.Entities;
using BadAPI.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BadAPI.Services
{
    public class ReviewService
    {
        private readonly IRepository<Review> _reviewRepo;
        private readonly IRepository<Product> _productRepo;
        private readonly IUnitOfWork _unitOfWork;

        public ReviewService(
            IRepository<Review> reviewRepo,
            IRepository<Product> productRepo,
            IUnitOfWork unitOfWork)
        {
            _reviewRepo = reviewRepo;
            _productRepo = productRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Review>> AddReviewAsync(
            Guid productId, string reviewerName, int rating, string comment)
        {
            var error = Validate(rating, comment, reviewerName);
            if (error is not null)
                return Result<Review>.Invalid(error);

            var product = await _productRepo.GetByIdAsync(productId);
            if (product == null)
                return Result<Review>.NotFound($"Product '{productId}' does not exist.");

            var review = new Review
            {
                ProductId = productId,
                ReviewerName = reviewerName.Trim(),
                Rating = rating,
                Comment = comment.Trim()
            };

            await _reviewRepo.AddAsync(review);
            await _unitOfWork.CompleteAsync();
            return Result<Review>.Success(review);
        }

        public async Task<Result<IEnumerable<Review>>> GetReviewsForProductAsync(Guid productId)
        {
            var product = await _productRepo.GetByIdAsync(productId);
            if (product == null)
                return Result<IEnumerable<Review>>.NotFound($"Product '{productId}' does not exist.");

            var reviews = await _reviewRepo.FindAsync(r => r.ProductId == productId);
            return Result<IEnumerable<Review>>.Success(reviews);
        }

        public async Task<Result<Review>> GetReviewAsync(Guid id)
        {
            var review = await _reviewRepo.GetByIdAsync(id);
            return review == null
                ? Result<Review>.NotFound("Review not found.")
                : Result<Review>.Success(review);
        }

        public async Task<Result<Review>> UpdateReviewAsync(Guid id, int rating, string comment)
        {
            var error = Validate(rating, comment);
            if (error is not null)
                return Result<Review>.Invalid(error);

            var review = await _reviewRepo.GetByIdAsync(id);
            if (review == null)
                return Result<Review>.NotFound("Review not found.");

            review.Rating = rating;
            review.Comment = comment.Trim();
            review.UpdatedAtUtc = DateTime.UtcNow;

            await _reviewRepo.UpdateAsync(review);
            await _unitOfWork.CompleteAsync();
            return Result<Review>.Success(review);
        }

        public async Task<Result> DeleteReviewAsync(Guid id)
        {
            var review = await _reviewRepo.GetByIdAsync(id);
            if (review == null)
                return Result.NotFound("Review not found.");

            await _reviewRepo.DeleteAsync(id);
            await _unitOfWork.CompleteAsync();
            return Result.Success();
        }

        private static string? Validate(int rating, string comment, string? reviewerName = null)
        {
            if (rating < 1 || rating > 5)
                return "Rating must be between 1 and 5.";
            if (string.IsNullOrWhiteSpace(comment))
                return "Comment cannot be empty.";
            if (reviewerName is not null && string.IsNullOrWhiteSpace(reviewerName))
                return "Reviewer name is required.";
            return null;
        }
    }
}
