using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce_app.Models.Products
{
    public class VariantValue : BaseEntity
    {
        public int Id { get; set; }

        public int VariantId { get; set; }
        public int OptionId { get; set; }
        public int OptionValueId { get; set; }

        public Variant? Variant { get; set; }
        public Option? Option { get; set; }
        public OptionValue? OptionValue { get; set; }
        
    }
}
