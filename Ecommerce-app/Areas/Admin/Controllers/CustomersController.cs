using Ecommerce_app.Areas.Identity.Data;
using Ecommerce_app.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce_app.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CustomersController : Controller
    {
        private readonly EcommerceAppContext _context;
        private readonly UserContext _userContext;

        public CustomersController(EcommerceAppContext context, UserContext userContext)
        {
            _context = context;
            _userContext = userContext;
        }
        public async Task<IActionResult> Index()
        {
            var users = await _userContext.Users.ToListAsync();
            return View(users);
        }
    }
}
