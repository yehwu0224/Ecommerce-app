using Ecommerce_app.Models.Orders;
using Ecommerce_app.Models.Products;

namespace Ecommerce_app.Models.ViewModels
{
    public class CartViewModel
    {
        public CartViewModel()
        {
            cartItems = new List<CartItem>();
            total = 0;
            coupon = "";
        }

        public List<CartItem>? cartItems { get; set; }
        public int total { get; set; }
        public string? coupon { get; set; }
    }

    public class CartItem
    {
        public string? SKU { get; set; }
        public int ProductId { get; set; } 
        public string? ProductName { get; set; }

        public int Quantity { get; set; }
        public int Price { get; set; }
        public int SubTotal { get; set; }

        public List<VariantValue>? VariantValues { get; set; }
        public string? imageStr { get; set; }
    }
}
