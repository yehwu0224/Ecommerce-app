using Ecommerce_app.Areas.Admin.Models.ViewModels;
using Ecommerce_app.Areas.Identity.Data;
using Ecommerce_app.Data;
using Ecommerce_app.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce_app.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class HomeController : Controller
    {
        private readonly EcommerceAppContext _context;
        private readonly UserContext _userContext;

        public HomeController(EcommerceAppContext context, UserContext userContext)
        {
            _context = context;
            _userContext = userContext;
        }

        public IActionResult Index()
        {
            var userNum = _userContext.Users.Count();
            var productNum = _context.Product.Count();
            var income = _context.Order.Select(x => x.TotalAmount).Sum();

            var revenue = _context.Order.Where(x => x.Created_at == DateTime.Today)
                            .Sum(x => x.TotalAmount);

            var recentlyOrder = _context.Order.OrderByDescending(x => x.OrderDate).Take(5).ToList();
            var products = _context.Product.OrderBy(x => x.Created_at).Take(4).ToList();
            var topSales = new List<ProductVM>();
            foreach(var item in products)
            {
                topSales.Add(new ProductVM { Id = item.Id, Name = item.Name, ImageStr = MyAppHelper.ViewImage(item.Image!) });
            }
            

            var dashboradVM = new DashboardViewModel()
            {
                UserNum = userNum,
                ProductNum = productNum,
                Income = income,
                Revenue = revenue,
                RecentlyOrder = recentlyOrder,
                TopSales = topSales
            };
            
            return View(dashboradVM);
        }
    }
}
