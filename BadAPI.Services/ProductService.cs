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
    public class ProductService
    {
        private ProductRepository _repo = new ProductRepository();

        // Business rule: price must be > 0
        public string AddProduct(Product product)
        {
            if (product.Price <= 0)
            {
                return "Price must be greater than zero";
            }

            _repo.Add(product);
            return "Product added";
        }

        public List<Product> GetProducts()
        {
            return _repo.GetAll();
        }

        // Business rule: cannot delete product if price > 100
        public string DeleteProduct(int id)
        {
            var product = _repo.GetById(id);
            if (product == null)
                return "Product not found";

            if (product.Price > 100)
                return "Cannot delete expensive products";

            _repo.Delete(id);
            return "Deleted";
        }
    }
}