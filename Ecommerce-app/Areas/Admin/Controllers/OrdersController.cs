using Ecommerce_app.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce_app.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrdersController : Controller
    {
        private readonly EcommerceAppContext _context;
        public OrdersController(EcommerceAppContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _context.Order.ToListAsync();
            return View(orders);
        }

        public async Task<IActionResult> Details(string orderId)
        {
            var order = await _context.Order
                .Include(x => x.OrderItems)
                .FirstOrDefaultAsync(x => x.OrderId == orderId);
            return View(order);
        }
    }
}
