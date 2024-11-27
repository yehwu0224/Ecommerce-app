using Ecommerce_app.Models.Orders;

namespace Ecommerce_app.Models.ViewModels
{
    public class CheckoutViewModel
    {
        public Order? Order { get; set; }
        public Dictionary<string,string>? EcpayOrder { get; set; }
    }
}
