using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce_app.Models.Products
{
    public class OptionValue : BaseEntity
    {
        public int Id { get; set; }
        public string? Value { get; set; }
        public byte[]? Image { get; set; }

        public int OptionId { get; set; }
        public Option? Option { get; set; }
    }
}
