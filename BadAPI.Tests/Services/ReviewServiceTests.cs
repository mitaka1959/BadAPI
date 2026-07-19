using BadAPI.Data.Entities;
using BadAPI.Data.Repositories;
using BadAPI.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BadAPI.Tests.Services
{
    public class ReviewServiceTests
    {
        private readonly Mock<IRepository<Review>> _reviewRepo = new();
        private readonly Mock<IRepository<Product>> _productRepo = new();
        private readonly Mock<IUnitOfWork> _unitOfWork = new();

        private ReviewService CreateSut() => new(
            _reviewRepo.Object, _productRepo.Object, _unitOfWork.Object);

        [Theory]
        [InlineData(0)]
        [InlineData(6)]
        [InlineData(-3)]
        public async Task AddReviewAsync_ReturnsInvalid_WhenRatingOutOfRange(int rating)
        {
            var result = await CreateSut().AddReviewAsync(Guid.NewGuid(), "Sam", rating, "Great");

            Assert.Equal(ResultStatus.Invalid, result.Status);
            _productRepo.Verify(r => r.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public async Task AddReviewAsync_ReturnsInvalid_WhenCommentEmpty(string comment)
        {
            var result = await CreateSut().AddReviewAsync(Guid.NewGuid(), "Sam", 4, comment);

            Assert.Equal(ResultStatus.Invalid, result.Status);
            _reviewRepo.Verify(r => r.AddAsync(It.IsAny<Review>()), Times.Never);
        }

        [Fact]
        public async Task AddReviewAsync_ReturnsNotFound_WhenProductDoesNotExist()
        {
            var productId = Guid.NewGuid();
            _productRepo.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync((Product?)null);

            var result = await CreateSut().AddReviewAsync(productId, "Sam", 4, "Solid");

            Assert.Equal(ResultStatus.NotFound, result.Status);
            _reviewRepo.Verify(r => r.AddAsync(It.IsAny<Review>()), Times.Never);
        }

        [Fact]
        public async Task AddReviewAsync_Succeeds_WhenValidAndProductExists()
        {
            var productId = Guid.NewGuid();
            _productRepo.Setup(r => r.GetByIdAsync(productId))
                        .ReturnsAsync(new Product { Id = productId });

            var result = await CreateSut().AddReviewAsync(productId, "Sam", 5, "Excellent");

            Assert.True(result.IsSuccess);
            Assert.Equal(productId, result.Value!.ProductId);
            _reviewRepo.Verify(r => r.AddAsync(It.Is<Review>(rv =>
                rv.ProductId == productId && rv.Rating == 5 && rv.Comment == "Excellent")), Times.Once);
            _unitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteReviewAsync_ReturnsNotFound_WhenMissing()
        {
            var id = Guid.NewGuid();
            _reviewRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Review?)null);

            var result = await CreateSut().DeleteReviewAsync(id);

            Assert.Equal(ResultStatus.NotFound, result.Status);
            _reviewRepo.Verify(r => r.DeleteAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task DeleteReviewAsync_Succeeds_WhenReviewExists()
        {
            var id = Guid.NewGuid();
            _reviewRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(new Review { Id = id });

            var result = await CreateSut().DeleteReviewAsync(id);

            Assert.True(result.IsSuccess);
            _reviewRepo.Verify(r => r.DeleteAsync(id), Times.Once);
            _unitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
        }
    }
}
