using System.Text;
using System.Threading;
using System.Runtime;
using System.Security;
using System.Timers;

namespace BadAPI.Data.Entities
{
    public class Category
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}