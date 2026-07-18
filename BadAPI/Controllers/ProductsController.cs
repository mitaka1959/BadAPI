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
    public class ProductsController : ControllerBase
    {
        private ProductService _service = new ProductService();
        private ProductRepository _repo = new ProductRepository();

        [HttpGet]
        public ActionResult Get()
        {
            var products = _service.GetProducts();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public ActionResult Get(int id)
        {
            var product = _repo.GetById(id);
            if (product == null)
            {
                return NotFound();
            }

            if (product.Price > 50)
            {
                return Ok(new { product.Id, product.Name, Discount = "10%" });
            }

            return Ok(product);
        }

        [HttpPost]
        public ActionResult Post(Product p)
        {
            var result = _service.AddProduct(p);
            if (result == "Price must be greater than zero")
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPut("{id}")]
        public ActionResult Put(int id, Product p)
        {
            if (id != p.Id)
                return BadRequest("Id mismatch");

            if (p.Price <= 0)
                return BadRequest("Invalid price");

            var existingProduct = _repo.GetById(id);
            if (existingProduct == null)
                return NotFound();

            existingProduct.Name = p.Name;
            existingProduct.Price = p.Price;
            existingProduct.CategoryId = p.CategoryId;
            existingProduct.CategoryName = p.CategoryName;

            _repo.Update(existingProduct);

            return Ok("Updated");
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var result = _service.DeleteProduct(id);

            if (result == "Product not found")
                return NotFound(result);

            if (result == "Cannot delete expensive products")
                return BadRequest(result);

            return Ok(result);
        }
    }
}
