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
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }

        public Guid CategoryId { get; set; }

        //nav
        [JsonIgnore]
        public Category? Category { get; set; } = null!;

        [NotMapped]
        public string InternalCode { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
    }
}