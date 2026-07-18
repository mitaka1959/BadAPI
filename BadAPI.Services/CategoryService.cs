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
    public class CategoryService
    {
        private readonly IRepository<Category> _repo;
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IRepository<Category> repo, IUnitOfWork unitOfWork)
        {
            _repo = repo;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Category>> AddCategoryAsync(string name, string description)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Result<Category>.Invalid("Category name is required.");

            var existing = await _repo.GetFirstOrDefaultAsync(c => c.Name == name);
            if (existing != null)
                return Result<Category>.Conflict($"Category '{name}' already exists.");

            var category = new Category { Name = name, Description = description };
            await _repo.AddAsync(category);
            await _unitOfWork.CompleteAsync();
            return Result<Category>.Success(category);
        }

        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            return await _repo.GetAllAsync();
        }
    }
}
