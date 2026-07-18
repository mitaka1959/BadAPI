using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BadAPI.Data.Repositories
{
    public interface IUnitOfWork
    {
        Task<int> CompleteAsync();
    }
}
