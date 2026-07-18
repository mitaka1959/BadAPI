using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Threading;
using System.Runtime;
using System.Security;
using System.Timers;

namespace BadAPI.Data.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }

        public int CategoryId { get; set; }
        public string CategoryName { get; set; }

        [NotMapped]
        public string InternalCode { get; set; }
    }
}