using BadApi.Data;
using BadApi.Services;
using BadAPI.Controllers;
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
    public class CategoriesController : ApiControllerBase
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
            return Ok(categories.Select(ToReadDto));
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CategoryCreateDto dto)
        {
            var result = await _service.AddCategoryAsync(dto.Name, dto.Description);
            if (!result.IsSuccess)
                return HandleFailure(result);

            var read = ToReadDto(result.Value!);
            return CreatedAtAction(nameof(Get), new { id = read.Id }, read);
        }

        private static CategoryReadDto ToReadDto(Category c) => new()
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description
        };
    }
}
