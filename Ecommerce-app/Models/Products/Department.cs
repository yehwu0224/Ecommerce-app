using System.ComponentModel.DataAnnotations;

namespace Ecommerce_app.Models.Products
{
    public class Department : BaseEntity
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = "new department";

        public virtual List<Category>? Categories { get; set; }
        public virtual List<Product>? Products { get; set; }

    }
}
