using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce_app.Models.Products
{
    public class Variant : BaseEntity
    {
        public int Id { get; set; }

        [Required]
        public string SKU { get; set; } = "new SKU";
        public byte[]? Image { get; set; }
        public int Stock { get; set; }

        public int ProductId { get; set; } // FK
        public Product? Product { get; set; } //反向導覽屬性

        [Timestamp]
        public uint Version { get; set; }

        public List<VariantValue>? VariantValues { get; set; }
    }
}
