using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce_app.Models.Products
{
    public class Option : BaseEntity
    {
        public int Id { get; set; }
        public string? Type { get; set; }

        public virtual List<OptionValue> OptionValues { get; set; } = [];
    }
}
