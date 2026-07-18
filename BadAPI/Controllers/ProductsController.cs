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
    public class ProductsController : ControllerBase
    {
        private readonly ProductService _service;

        public ProductsController(ProductService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var products = await _service.GetProductsAsync();
            return Ok(products); 
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ProductCreateDto dto)
        {
            var newProduct = new Product
            {
                Name = dto.Name,
                Price = dto.Price,
                CategoryName = dto.CategoryName
            };

            var result = await _service.AddProductAsync(newProduct);

            if (result == "Product added successfully")
            {
                return Ok(new { message = result });
            }

            return BadRequest(new { message = result });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _service.DeleteProductAsync(id);

            if (result == "Deleted")
                return Ok(new { message = result });

            if (result == "Product not found")
                return NotFound(new { message = result }); 

            return BadRequest(new { message = result });
        }
    }
}
