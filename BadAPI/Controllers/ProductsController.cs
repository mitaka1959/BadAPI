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
    public class ProductsController : ApiControllerBase
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
            return Ok(products.Select(ToReadDto));
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ProductCreateDto dto)
        {
            var result = await _service.AddProductAsync(dto.Name, dto.Price, dto.CategoryName);
            if (!result.IsSuccess)
                return HandleFailure(result);

            var read = ToReadDto(result.Value!);
            return CreatedAtAction(nameof(Get), new { id = read.Id }, read);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _service.DeleteProductAsync(id);
            return result.IsSuccess ? NoContent() : HandleFailure(result);
        }

        private static ProductReadDto ToReadDto(Product p) => new()
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            CategoryId = p.CategoryId
        };
    }
}
