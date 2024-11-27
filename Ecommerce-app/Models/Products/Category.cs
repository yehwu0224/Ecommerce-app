using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce_app.Models.Products
{
    public class Category : BaseEntity
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = "new category";
        public int DepartmentId { get; set; }
        public virtual List<Product>? Products { get; set; }
    }
}
