using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime;
using System.Security;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using System.Timers;

namespace BadAPI.Data.Entities
{
    public class Product
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public string InternalCode { get; set; } = Guid.NewGuid().ToString();

        public Guid CategoryId { get; set; }

        [JsonIgnore]
        public Category? Category { get; set; }

        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}