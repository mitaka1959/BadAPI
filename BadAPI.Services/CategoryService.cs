using System.Collections.Generic;
using BadApi.Data;
using BadApi.Repositories;
using BadAPI.Data.Entities;
using System.Text;
using System.Threading;
using System.Runtime;
using System.Security;
using System.Timers;

namespace BadApi.Services
{
    public class CategoryService
    {
        private CategoryRepository _repo = new CategoryRepository();

        // Business rule: Name cannot be empty
        public string AddCategory(Category category)
        {
            if (string.IsNullOrEmpty(category.Name))
            {
                return "Category name is required";
            }

            _repo.Add(category);
            return "Category added";
        }

        public List<Category> GetCategories()
        {
            return _repo.GetAll();
        }
    }
}
