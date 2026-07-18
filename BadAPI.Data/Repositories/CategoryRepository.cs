using System;
using System.Collections.Generic;
using System.Linq;
using BadApi.Data;
using BadAPI.Data.Entities;

namespace BadApi.Repositories
{
    public class CategoryRepository
    {
        private readonly BadDbContext _context;

        public CategoryRepository(BadDbContext context)
        {
            _context = context;
        }

        public List<Category> GetAll()
        {
            return _context.Set<Category>().ToList();
        }

        public Category? GetById(Guid id)
        {
            return _context.Set<Category>().FirstOrDefault(c => c.Id == id);
        }

        public void Add(Category category)
        {
            _context.Set<Category>().Add(category);
            _context.SaveChanges();
        }

        public void Update(Category category)
        {
            var c = _context.Set<Category>().FirstOrDefault(x => x.Id == category.Id);
            if (c != null)
            {
                c.Name = category.Name;
                c.Description = category.Description;
                _context.SaveChanges();
            }
        }

        public void Delete(Guid id)
        {
            var c = _context.Set<Category>().FirstOrDefault(x => x.Id == id);
            if (c != null)
            {
                _context.Set<Category>().Remove(c);
                _context.SaveChanges();
            }
        }
    }
}