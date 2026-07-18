using BadApi.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BadAPI.Data.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BadDbContext _context;

        public UnitOfWork(BadDbContext context)
        {
            _context = context;
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
