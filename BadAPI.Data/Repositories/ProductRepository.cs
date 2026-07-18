using System;
using System.Collections.Generic;
using System.Linq;
using BadApi.Data;
using BadAPI.Data.Entities;

namespace BadApi.Repositories
{
    public class ProductRepository
    {
        private BadDbContext _context = new BadDbContext();

        public List<Product> GetAll()
        {
            return _context.Products.ToList();
        }

        public Product GetById(int id)
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
                p.CategoryName = product.CategoryName;

                _context.SaveChanges();
            }
        }

        public void Delete(int id)
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