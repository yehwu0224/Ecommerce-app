using Ecommerce_app.Models.Products;

namespace Ecommerce_app.Models.Orders
{
    public class OrderItem : BaseEntity
    {
        public int Id { get; set; }

        public string? SKU { get; set; }
        public int ProductId { get; set; } // FK
        public string? ProductName { get; set; }

        public int Quantity { get; set; }
        public int Price { get; set; }
        public int SubTotal { get; set; }

        //public virtual List<VariantValue>? VariantValues { get; set; }

        public string? OrderId { get; set; }
    }
}
