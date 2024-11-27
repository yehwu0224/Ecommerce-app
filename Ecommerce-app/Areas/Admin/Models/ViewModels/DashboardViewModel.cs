using Ecommerce_app.Models.Orders;
using Ecommerce_app.Models.Products;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce_app.Areas.Admin.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int UserNum { get; set; }
        public int ProductNum { get; set; }
        public int Income {  get; set; }
        public int Revenue { get; set; }
        public List<Order>? RecentlyOrder { get; set; }
        public List<ProductVM>? TopSales { get; set; }
    }

    public class ProductVM
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? ImageStr { get; set; }
    }
}
