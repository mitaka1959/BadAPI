using BadApi.Services;
using BadAPI.Data.Entities;
using BadAPI.Data.Repositories;
using BadAPI.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace BadAPI.Tests.Services
{
    public class ProductServiceTests
    {

        private readonly Mock<IRepository<Product>> _productRepo = new();
        private readonly Mock<IRepository<Category>> _categoryRepo = new();
        private readonly Mock<IRepository<Review>> _reviewRepo = new();
        private readonly Mock<IUnitOfWork> _unitOfWork = new();

        private ProductService CreateSut() => new(
            _productRepo.Object, _categoryRepo.Object, _reviewRepo.Object, _unitOfWork.Object);


        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-0.01)]
        public async Task AddProductAsync_ReturnsInvalid_WhenPriceNotPositive(decimal price)
        {
            var result = await CreateSut().AddProductAsync("Widget", price, "Tools");

            Assert.Equal(ResultStatus.Invalid, result.Status);
            _categoryRepo.Verify(r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Category, bool>>>()), Times.Never);
            _unitOfWork.Verify(u => u.CompleteAsync(), Times.Never);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task AddProductAsync_ReturnsInvalid_WhenCategoryNameMissing(string? categoryName)
        {
            var result = await CreateSut().AddProductAsync("Widget", 10m, categoryName!);

            Assert.Equal(ResultStatus.Invalid, result.Status);
            _productRepo.Verify(r => r.AddAsync(It.IsAny<Product>()), Times.Never);
        }

        [Fact]
        public async Task AddProductAsync_ReturnsNotFound_WhenCategoryDoesNotExist()
        {
            _categoryRepo.Setup(r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Category, bool>>>()))
                .ReturnsAsync((Category?)null);

            var result = await CreateSut().AddProductAsync("Widget", 10m, "Ghost");

            Assert.Equal(ResultStatus.NotFound, result.Status);
            _productRepo.Verify(r => r.AddAsync(It.IsAny<Product>()), Times.Never);
            _unitOfWork.Verify(u => u.CompleteAsync(), Times.Never);
        }

        [Fact]
        public async Task AddProductAsync_Succeeds_AndLinksCategory_WhenValid()
        {
            var category = new Category { Id = Guid.NewGuid(), Name = "Tools" };
            _categoryRepo.Setup(r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Category, bool>>>()))
                .ReturnsAsync(category);

            var result = await CreateSut().AddProductAsync("Widget", 10m, "Tools");

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(category.Id, result.Value!.CategoryId);   
            _productRepo.Verify(r => r.AddAsync(It.Is<Product>(p =>
                p.Name == "Widget" && p.Price == 10m && p.CategoryId == category.Id)), Times.Once);
            _unitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
        }


        [Fact]
        public async Task DeleteProductAsync_ReturnsNotFound_WhenProductMissing()
        {
            var id = Guid.NewGuid();
            _productRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Product?)null);

            var result = await CreateSut().DeleteProductAsync(id);

            Assert.Equal(ResultStatus.NotFound, result.Status);
            _productRepo.Verify(r => r.DeleteAsync(It.IsAny<Guid>()), Times.Never);
            _unitOfWork.Verify(u => u.CompleteAsync(), Times.Never);
        }

        [Fact]
        public async Task DeleteProductAsync_ReturnsConflict_WhenPriceAbove50()
        {
            var id = Guid.NewGuid();
            _productRepo.Setup(r => r.GetByIdAsync(id))
                        .ReturnsAsync(new Product { Id = id, Price = 75m });

            var result = await CreateSut().DeleteProductAsync(id);

            Assert.Equal(ResultStatus.Conflict, result.Status);
            Assert.Contains("$50", result.Error!);
            _reviewRepo.Verify(r => r.AnyAsync(
                It.IsAny<Expression<Func<Review, bool>>>()), Times.Never);
            _productRepo.Verify(r => r.DeleteAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task DeleteProductAsync_AllowsPriceExactly50_Boundary()
        {
        
            var id = Guid.NewGuid();
            _productRepo.Setup(r => r.GetByIdAsync(id))
                        .ReturnsAsync(new Product { Id = id, Price = 50m });
            _reviewRepo.Setup(r => r.AnyAsync(
                It.IsAny<Expression<Func<Review, bool>>>())).ReturnsAsync(false);

            var result = await CreateSut().DeleteProductAsync(id);

            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task DeleteProductAsync_ReturnsConflict_WhenProductHasReviews()
        {
            var id = Guid.NewGuid();
            _productRepo.Setup(r => r.GetByIdAsync(id))
                        .ReturnsAsync(new Product { Id = id, Price = 20m });
            _reviewRepo.Setup(r => r.AnyAsync(
                It.IsAny<Expression<Func<Review, bool>>>())).ReturnsAsync(true);

            var result = await CreateSut().DeleteProductAsync(id);

            Assert.Equal(ResultStatus.Conflict, result.Status);
            Assert.Contains("reviews", result.Error!);
            _productRepo.Verify(r => r.DeleteAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task DeleteProductAsync_Succeeds_WhenCheapAndNoReviews()
        {
            var id = Guid.NewGuid();
            _productRepo.Setup(r => r.GetByIdAsync(id))
                        .ReturnsAsync(new Product { Id = id, Price = 20m });
            _reviewRepo.Setup(r => r.AnyAsync(
                It.IsAny<Expression<Func<Review, bool>>>())).ReturnsAsync(false);

            var result = await CreateSut().DeleteProductAsync(id);

            Assert.True(result.IsSuccess);
            _productRepo.Verify(r => r.DeleteAsync(id), Times.Once);
            _unitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
        }


        [Fact]
        public async Task GetProductsAsync_ReturnsAllFromRepository()
        {
            var products = new List<Product> { new(), new() };
            _productRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(products);

            var result = await CreateSut().GetProductsAsync();

            Assert.Equal(2, result.Count());
        }
    }
}
