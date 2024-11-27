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

        public async Task<IActionResult> Index(string? filter)
        {
            var products = await _context.Product
                            .Include(e => e.Department)
                            .Include(e => e.Category)
                            .Include(e => e.Variants)!
                                .ThenInclude(e => e.VariantValues)!
                                    .ThenInclude(e => e.Option)
                            .Include(e => e.Variants)!
                                .ThenInclude(e => e.VariantValues)!
                                    .ThenInclude(e => e.OptionValue)
                            .ToListAsync();
                                

            if (!String.IsNullOrEmpty(filter))
            {
                products = products.Where(s => s.Department!.Name == filter).ToList();
                ViewData["Head"] = filter;
            }
            else
            {
                ViewData["Head"] = "所有商品";
            }

            var viewModel = new List<IndexViewModel>();
            foreach(var item in products)
            {
                // 取得一個商品的所有顏色選項（ 下面以Distinct()去除重複 ）
                var colors = new List<string>();
                foreach (var variant in item.Variants!)
                {
                    var image = variant.VariantValues?.Find(x => x.Option?.Type == "顏色")!.OptionValue!.Image;
                    if(image != null)
                    {
                        colors.Add(MyAppHelper.ViewImage(image));
                    }
                }

                // 產品描述format
                if (item.Description != null){
                    var maxlength = item.Description.Length < 42 ? item.Description.Length : 42;
                    item.Description = maxlength < 42 ? 
                        item.Description.Substring(0, maxlength) : item.Description.Substring(0, maxlength) + "...";
                }
                
                var viewModelItem = new IndexViewModel()
                {
                    Id = item.Id,
                    Name = item.Name,
                    Description = item.Description,
                    Price = item.Price,
                    ImageStr = MyAppHelper.ViewImage(item.Image!),
                    Department = item.Department!.Name,
                    Category = item.Category!.Name,
                    Colors = colors.Distinct().ToList()
                };
                viewModel.Add(viewModelItem);
            }

            return View(viewModel);
        }

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
            else
            {
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
            }

            return View(productVM);
        }

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
