using BadApi.Data;
using BadApi.Repositories;
using BadAPI.Data.Entities;
using BadAPI.Data.Repositories;
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
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IRepository<Product> productRepo, IRepository<Category> categoryRepo, IUnitOfWork unitOfWork)
        {
            _productRepo = productRepo;
            _categoryRepo = categoryRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<string> AddProductAsync(Product product)
        {
            if (product.Price <= 0)
                return "Price must be greater than zero";

            if (string.IsNullOrEmpty(product.CategoryName))
                return "CategoryName is required in the JSON payload";

            var category = await _categoryRepo.GetFirstOrDefaultAsync(c => c.Name == product.CategoryName);

            if (category == null)
                return $"Category '{product.CategoryName}' does not exist. Please create it first.";

            product.CategoryId = category.Id;

            await _productRepo.AddAsync(product);
            await _unitOfWork.CompleteAsync();
            return "Product added successfully";
        }

        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            return await _productRepo.GetAllAsync();
        }

        public async Task<string> DeleteProductAsync(Guid id) 
        {
            var product = await _productRepo.GetByIdAsync(id);
            if (product == null)
                return "Product not found";

            if (product.Price > 100)
                return "Cannot delete expensive products";

            await _productRepo.DeleteAsync(id);
            await _unitOfWork.CompleteAsync();
            return "Deleted";
        }
    }
}