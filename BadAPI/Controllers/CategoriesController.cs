using BadApi.Data;
using BadApi.Repositories;
using BadApi.Services;
using BadAPI.Data.Entities;
using BadAPI.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Runtime;
using System.Security;
using System.Text;
using System.Threading;
using System.Timers;

namespace BadApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly CategoryService _service;

       
        public CategoriesController(CategoryService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            
            var categories = await _service.GetCategoriesAsync();

            return Ok(categories);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CategoryCreateDto dto)
        {
        
            var newCategory = new Category
            {
                Name = dto.Name,
                Description = dto.Description
            };

            var result = await _service.AddCategoryAsync(newCategory);

            if (result == "Category added")
            {
                return Ok(new { message = result });
            }

            return BadRequest(new { message = result });
        }
    }
}
