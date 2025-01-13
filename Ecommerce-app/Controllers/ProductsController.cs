using System.ComponentModel.DataAnnotations;
using Ecommerce_app.Areas.Identity.Models;
using Ecommerce_app.Data;
using Ecommerce_app.Helpers;
using Ecommerce_app.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;
using StackExchange.Redis;

namespace Ecommerce_app.Controllers
{
    public class ProductsController : Controller
    {
        private readonly EcommerceAppContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IConnectionMultiplexer _connectionMultiplexer;

        public ProductsController(EcommerceAppContext context, UserManager<AppUser> userManager, IConnectionMultiplexer multiplexer)
        {
            _context = context;
            _userManager = userManager;
            _connectionMultiplexer = multiplexer;
        }

        /// <summary>
        /// 產品列表頁面
        /// </summary>
        /// <param name="filter">篩選條件</param>
        /// <returns>產品列表頁面</returns>
        public async Task<IActionResult> Index(string? filter)
        {
            // 建立產品查詢
            var query = _context.Product
                            .Include(e => e.Department)
                            .Include(e => e.Category)
                            .Include(e => e.Variants)!
                                .ThenInclude(e => e.VariantValues)!
                                    .ThenInclude(e => e.Option)
                            .Include(e => e.Variants)!
                                .ThenInclude(e => e.VariantValues)!
                                    .ThenInclude(e => e.OptionValue)
                            .AsQueryable();

            // 應用篩選條件
            if (!String.IsNullOrEmpty(filter))
            {
                query = query.Where(s => s.Department!.Name == filter);
                ViewData["Head"] = filter; // 設定頁面標題
            }
            else
            {
                ViewData["Head"] = "所有商品";
            }

            // 執行查詢並取得結果
            var products = await query.ToListAsync();

            // 建立視圖模型
            var viewModel = products.Select(item => new IndexViewModel
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description?.Length > 42 ? item.Description.Substring(0, 42) + "..." : item.Description,
                Price = item.Price,
                ImageStr = MyAppHelper.ViewImage(item.Image!),
                Department = item.Department!.Name,
                Category = item.Category!.Name,
                // 取得顏色選項
                Colors = item.Variants!.SelectMany(v => v.VariantValues!)
                                    .Where(v => v.Option?.Type == "顏色")
                                    .Select(v => MyAppHelper.ViewImage(v.OptionValue!.Image!))
                                    .Distinct()
                                    .ToList() 
            }).ToList();

            return View(viewModel);
        }

        /// <summary>
        /// 顯示產品詳細資料頁面。
        /// </summary>
        /// <param name="id">產品編號</param>
        /// <returns>產品詳細資料頁面</returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            DetailsViewModel productVM = new DetailsViewModel();

            var product = await _context.Product
                .Include(e => e.Department)
                .Include(e => e.Category)
                .Include(e => e.Variants)!
                    .ThenInclude(e => e.VariantValues)!
                        .ThenInclude(e => e.Option)
                .Include(e => e.Variants)!
                    .ThenInclude(e => e.VariantValues)!
                        .ThenInclude(e => e.OptionValue)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            var album = new List<string>();
            var colors = new List<ColorModel>();
            foreach (var variant in product.Variants!)
            {
                var image1 = variant.Image!;
                var colorImage = variant.VariantValues?.Find(x => x.Option?.Type == "顏色")!.OptionValue!.Image;
                var colorValue = variant.VariantValues?.Find(x => x.Option?.Type == "顏色")!.OptionValue!.Value;
                if (colorImage != null)
                {
                    var color = new ColorModel()
                    {
                        Value = colorValue,
                        ImageStr = MyAppHelper.ViewImage(colorImage)
                    };
                    colors.Add(color);
                    album.Add(MyAppHelper.ViewImage(image1));
                }
            }

            productVM = new DetailsViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Department = product.Department!.Name,
                Description = product.Description,
                Content = product.Content,
                Price = product.Price,
                ImageStr = MyAppHelper.ViewImage(product.Image!),
                Colors = colors.DistinctBy(x => x.Value).ToList(),
                Album = album.Distinct().ToList()
            };

            return View(productVM);
        }

        /// <summary>
        /// 取得指定顏色和尺寸的庫存編號。
        /// </summary>
        /// <param name="id">產品編號</param>
        /// <param name="color">顏色</param>
        /// <param name="size">尺寸</param>
        /// <returns>庫存編號</returns>
        public async Task<IActionResult> GetSKU(int? id, string color, string size)
        {
            var variants = await _context.Variant.Where( x => x.ProductId == id)
                .Include(e => e.VariantValues)!
                    .ThenInclude(e => e.Option)
                .Include(e => e.VariantValues)!
                    .ThenInclude(e => e.OptionValue).ToListAsync();

            var filter1 = variants.Where(x => x.VariantValues!.Any(s => s.Option?.Type == "顏色" && s.OptionValue?.Value == color)).ToList();
            var filter2 = filter1.Where(x => x.VariantValues!.Any(s => s.Option?.Type == "尺寸" && s.OptionValue?.Value == size)).ToList();

            string sku = "尚無庫存";
            if(filter2.Count != 0)
            {
                sku = filter2.Select(x => x.SKU).FirstOrDefault()!.ToString();
            }

            return new JsonResult(sku);
        }

    }
}
