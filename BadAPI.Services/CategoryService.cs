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
    public class CategoryService
    {
        private readonly IRepository<Category> _repo;
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IRepository<Category> repo, IUnitOfWork unitOfWork)
        {
            _repo = repo;
            _unitOfWork = unitOfWork;
        }

        public async Task<string> AddCategoryAsync(Category category)
        {
            if (string.IsNullOrEmpty(category.Name))
            {
                return "Category name is required";
            }

            await _repo.AddAsync(category);

            await _unitOfWork.CompleteAsync();

            return "Category added";
        }

        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            return await _repo.GetAllAsync();
        }
    }
}
