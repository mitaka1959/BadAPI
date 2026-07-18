using Microsoft.AspNetCore.Mvc;
using BadApi.Services;
using BadApi.Repositories;
using BadApi.Data;
using BadAPI.Data.Entities;
using System.Text;
using System.Threading;
using System.Runtime;
using System.Security;
using System.Timers;

namespace BadApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private CategoryService _service = new CategoryService();
        private CategoryRepository _repo = new CategoryRepository();

        [HttpGet]
        public ActionResult Get()
        {
            var categories = _service.GetCategories();
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public ActionResult Get(int id)
        {
            var category = _repo.GetById(id);
            if (category == null)
            {
                return NotFound();
            }

            if (category.Description != null && category.Description.Length > 100)
            {
                return Ok(new { category.Id, category.Name, Note = "Long description" });
            }

            return Ok(category);
        }

        [HttpPost]
        public ActionResult Post(Category category)
        {
            var result = _service.AddCategory(category);
            if (result == "Category name is required")
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPut("{id}")]
        public ActionResult Put(int id, Category category)
        {
            if (id != category.Id)
                return BadRequest("Id mismatch");

            var existing = _repo.GetById(id);
            if (existing == null)
                return NotFound();

            existing.Name = category.Name;
            existing.Description = category.Description;

            _repo.Update(existing);

            return Ok("Updated");
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var category = _repo.GetById(id);
            if (category == null)
                return NotFound();

            _repo.Delete(id);

            return Ok("Deleted");
        }
    }
}
