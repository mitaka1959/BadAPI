using System;
using System.Collections.Generic;
using System.Linq;
using BadApi.Data;
using BadAPI.Data.Entities;

namespace BadApi.Repositories
{
    public class ProductRepository
    {
        private readonly BadDbContext _context;

        public ProductRepository(BadDbContext context)
        {
            _context = context;
        }

        public List<Product> GetAll()
        {
            return _context.Products.ToList();
        }

        public Product? GetById(Guid id)
        {
            return _context.Products.FirstOrDefault(x => x.Id == id);
        }

        public void Add(Product product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();
        }

        public void Update(Product product)
        {
            var p = _context.Products.FirstOrDefault(x => x.Id == product.Id);
            if (p != null)
            {
                p.Name = product.Name;
                p.Price = product.Price;
                p.CategoryId = product.CategoryId;

                _context.SaveChanges();
            }
        }

        public void Delete(Guid id)
        {
            var p = _context.Products.FirstOrDefault(x => x.Id == id);
            if (p != null)
            {
                _context.Products.Remove(p);
                _context.SaveChanges();
            }
        }
    }
}