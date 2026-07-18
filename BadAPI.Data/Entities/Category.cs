using System.Text;
using System.Threading;
using System.Runtime;
using System.Security;
using System.Timers;

namespace BadAPI.Data.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}