using Ecommerce_app.Areas.Admin.Models.ViewModels;
using Ecommerce_app.Data;
using Ecommerce_app.Helpers;
using Ecommerce_app.Models.Products;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.IO;

namespace Ecommerce_app.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductsController : Controller
    {
        private readonly EcommerceAppContext _context;

        public ProductsController(EcommerceAppContext context)
        {
            _context = context;
        }
        
        public async Task<IActionResult> Index(string? searchString)
        {
            var product = await _context.Product.Include(x => x.Department)
                                .Include(x => x.Category)
                                .Include(x => x.Variants)
                                .ToListAsync();
            if (!String.IsNullOrEmpty(searchString))
            {
                product = product.Where(s => s.Name.Contains(searchString)).ToList();
            }

            List<ProductListViewModel> viewModel = new List<ProductListViewModel>();
            foreach(var item in product)
            {
                ProductListViewModel viewModelItem = new ProductListViewModel()
                {
                    Id = item.Id,
                    Name = item.Name,
                    Department = item.Department!.Name,
                    Category = item.Category!.Name,
                    Count = item.Variants!.Count(),
                    ImageStr = MyAppHelper.ViewImage(item.Image!),
                    Created_at = item.Created_at,
                    Modiftied_at = item.Modiftied_at
                };
                viewModel.Add(viewModelItem);
            }
            return View(viewModel);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewBag.departments = _context.Department.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Name,
            }).ToList();

            ViewBag.attributes = _context.Option.ToList();

            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Content,Price,Image,DepartmentId,CategoryId,Options")] Product product, IFormFile myimg, List<int> attributes)
        {
            if (ModelState.IsValid)
            {
                if (myimg != null)
                {
                    product.Image = MyAppHelper.ImageToByteArray(myimg);
                }
                var options = _context.Option.Where(x => attributes.Contains(x.Id)).ToList();
                product.Options = options;

                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ProductViewModel productVM = new ProductViewModel();
            //productVM.Product = new Product();
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            else
            {
                productVM = new ProductViewModel
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Content = product.Content,
                    Price = product.Price,
                    ImageStr = MyAppHelper.ViewImage(product.Image!),
                    Created_at = product.Created_at,
                    Modiftied_at = product.Modiftied_at
                };
                /*
                productVM.Product = product;
                if (product.Image != null)
                {
                    productVM.ImageStr = MyAppHelper.ViewImage(product.Image);
                }*/
            }

            return View(productVM);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)

        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product.FindAsync(id);
            if (product == null)
            {
                
                return NotFound();
            }
            else
            {
                ViewData["imageStr"] = MyAppHelper.ViewImage(product.Image!);
                ViewBag.attributes = _context.Option.ToList();
                ViewBag.departments = await _context.Department.Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name,
                }).ToListAsync();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product, IFormFile? myimg, List<int> attributes)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // 如果沒有上傳圖片，則不更新圖片欄位
                    if (myimg == null)
                    {
                        _context.Entry(product).Property(p => p.Image).IsModified = false;
                    }

                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Product.FindAsync(id);
            if (product != null)
            {
                _context.Product.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Product.Any(e => e.Id == id);
        }

        public JsonResult OnGetCategories(int dep_id)
        {
            var categories = _context.Category.Where(x => x.DepartmentId == dep_id).Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Name,
            }).ToList();

            return new JsonResult(categories);
        }
    }
}
