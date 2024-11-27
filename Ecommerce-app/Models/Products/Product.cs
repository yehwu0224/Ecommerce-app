using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace Ecommerce_app.Models.Products
{
    public class Product : BaseEntity
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = "new product";
        public string? Description { get; set; }
        public string? Content { get; set; }
        public int Price { get; set; }
        public byte[]? Image { get; set; }

        // 反向導覽屬性
        public int DepartmentId { get; set; }
        public virtual Department? Department { get; set; }
        public int CategoryId { get; set; }
        public virtual Category? Category { get; set; }

        // 參考導覽屬性
        public virtual List<Variant>? Variants { get; set; } 
        public virtual List<Option>? Options { get; set; }
        public virtual List<Comment>? Comments { get; set; } 
    }
}
