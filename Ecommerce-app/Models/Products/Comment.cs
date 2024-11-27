using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce_app.Models.Products
{
    public class Comment : BaseEntity
    {
        public int Id { get; set; }
        public string? UserName { get; set; }
        public string? Content { get; set; }
        public int ProductId { get; set; }
        public virtual Product? Product { get; set; }
    }
}
