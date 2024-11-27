using Ecommerce_app.Areas.Identity.Models;
using Ecommerce_app.Data;
using Ecommerce_app.Helpers;
using Ecommerce_app.Models.Orders;
using Ecommerce_app.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Differencing;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NuGet.Protocol;
using StackExchange.Redis;
using System.Security.Cryptography.Xml;


namespace Ecommerce_app.Controllers
{
    //[Authorize]
    public class CartController : Controller
    {
        private readonly EcommerceAppContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IConnectionMultiplexer _connectionMultiplexer;

        public CartController(EcommerceAppContext context, UserManager<AppUser> userManager, IConnectionMultiplexer multiplexer)
        {
            _context = context;
            _userManager = userManager;
            _connectionMultiplexer = multiplexer;
        }

        public IActionResult Index()
        {
            if (HttpContext.User.Identity?.IsAuthenticated == false)
            {
                return RedirectToAction("Login", "Account", new { area = "Identity", returnUrl = Request.Path });
            }

            var userId = _userManager.GetUserId(User);
            var myCart = new CartViewModel();

            var data = _connectionMultiplexer.GetDatabase(0).HashGetAll(userId).ToList();

            foreach (var item in data)
            {
                var sss = _context.Variant?
                    .Include(x => x.Product)
                    .Include(x => x.VariantValues)!
                        .ThenInclude(x => x.Option)
                    .Include(x => x.VariantValues)!
                        .ThenInclude(x => x.OptionValue)
                    .FirstOrDefault(e => e.SKU == item.Name.ToString());
                CartItem cartItem = new CartItem();
                if(sss != null)
                {
                    cartItem.SKU = sss.SKU;
                    cartItem.ProductId= sss.ProductId;
                    cartItem.ProductName = sss.Product?.Name;
                    cartItem.Price = sss.Product != null ? sss.Product.Price : 0;
                    cartItem.VariantValues = sss.VariantValues;
                    cartItem.Quantity = ((int)item.Value);
                    cartItem.SubTotal = cartItem.Quantity * cartItem.Price;
                    cartItem.imageStr = MyAppHelper.ViewImage(sss.Image!);
                    myCart.cartItems?.Add(cartItem);
                    myCart.total += cartItem.SubTotal;
                }
            }
            
            return View(myCart);
        }

        [HttpPost]
        [Route("api/Cart/AddtoCart")]
        public async Task<IActionResult> AddtoCart(string sku, int quantity)
        {
            var url = Request.Path;
            if (HttpContext.User.Identity?.IsAuthenticated == false)
            {
                return BadRequest("請先登入帳號");
                //return RedirectToAction("Login", "Account", new { area = "Identity", returnUrl = url });
            }

            var userId = _userManager.GetUserId(User);
            var item = await _context.Variant.Where(x => x.SKU == sku).FirstOrDefaultAsync();

            try
            {
                if (item == null)
                {
                    throw new ArgumentException("商品不存在");
                }
                else if (quantity > item.Stock)
                {
                    throw new ArgumentException("商品庫存不足");
                }
                else
                {
                    var data = await _connectionMultiplexer.GetDatabase(0).HashGetAsync(userId, sku); // 檢查購物車內有無相同物品
                    if (data == 0)
                    {
                        await _connectionMultiplexer.GetDatabase(0).HashSetAsync(userId, new HashEntry[] { new HashEntry(sku, quantity) });
                    }
                    else
                    {
                        await _connectionMultiplexer.GetDatabase(0).HashSetAsync(userId, new HashEntry[] { new HashEntry(sku, quantity + (int)data) });
                    }
                    return Ok();
                }
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public async Task<IActionResult> RemoveItem (string sku, int quantity)
        {
            var userId = _userManager.GetUserId(User);
            await _connectionMultiplexer.GetDatabase(0).HashDeleteAsync(userId, sku); // 移除指定商品

            return RedirectToAction("Index");
        }

    }
}
