using BadApi.Data;
using BadAPI.Data.Entities;
using BadAPI.Data.Repositories;
using BadAPI.Services;
using System.Collections.Generic;
using System.Runtime;
using System.Security;
using System.Text;
using System.Threading;
using System.Timers;

namespace BadApi.Services
{
    public class ProductService
    {
        private readonly IRepository<Product> _productRepo;
        private readonly IRepository<Category> _categoryRepo;
        private readonly IRepository<Review> _reviewRepo;
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(
            IRepository<Product> productRepo,
            IRepository<Category> categoryRepo,
            IRepository<Review> reviewRepo,
            IUnitOfWork unitOfWork)
        {
            _productRepo = productRepo;
            _categoryRepo = categoryRepo;
            _reviewRepo = reviewRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Product>> AddProductAsync(string name, decimal price, string categoryName)
        {
            if (price <= 0)
                return Result<Product>.Invalid("Price must be greater than zero.");

            if (string.IsNullOrWhiteSpace(categoryName))
                return Result<Product>.Invalid("CategoryName is required.");

            var category = await _categoryRepo.GetFirstOrDefaultAsync(c => c.Name == categoryName);
            if (category == null)
                return Result<Product>.NotFound(
                    $"Category '{categoryName}' does not exist. Please create it first.");

            var product = new Product
            {
                Name = name,
                Price = price,
                CategoryId = category.Id
            };

            await _productRepo.AddAsync(product);
            await _unitOfWork.CompleteAsync();
            return Result<Product>.Success(product);
        }

        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            return await _productRepo.GetAllAsync();
        }

        public async Task<Result> DeleteProductAsync(Guid id)
        {
            var product = await _productRepo.GetByIdAsync(id);
            if (product == null)
                return Result.NotFound("Product not found.");

            if (product.Price > 50m)
                return Result.Conflict(
                    $"Product cannot be deleted: its price (${product.Price:0.##}) exceeds the $50 limit.");

            if (await _reviewRepo.AnyAsync(r => r.ProductId == id))
                return Result.Conflict(
                    "Product cannot be deleted because it has customer reviews.");

            await _productRepo.DeleteAsync(id);
            await _unitOfWork.CompleteAsync();
            return Result.Success();
        }
    }
}