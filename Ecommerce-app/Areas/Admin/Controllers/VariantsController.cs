using Ecommerce_app.Areas.Admin.Models.ViewModels;
using Ecommerce_app.Data;
using Ecommerce_app.Helpers;
using Ecommerce_app.Models.Products;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce_app.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class VariantsController : Controller
    {
        private readonly EcommerceAppContext _context;

        public VariantsController(EcommerceAppContext context)
        {
            _context = context;
        }

        // GET: Variants
        public async Task<IActionResult> Index()
        {
            var skus = await _context.Variant.Include(v => v.Product).ToListAsync();
            var viewModel = new List<VariantListViewModel>();
            foreach(var item in skus)
            {
                var viewModelItem = new VariantListViewModel()
                {
                    Id = item.Id,
                    SKU = item.SKU,
                    Product = item.Product!.Name,
                    Stock = item.Stock,
                    Image = MyAppHelper.ViewImage(item.Image!),
                    Created_at = item.Created_at,
                    Modiftied_at = item.Modiftied_at
                };
                viewModel.Add(viewModelItem);
            }
            return View(viewModel);
        }

        // GET: Variants/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var variant = await _context.Variant
                .Include(v => v.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (variant == null)
            {
                return NotFound();
            }

            return View(variant);
        }

        // GET: Variants/Create
        public async Task<IActionResult> Create(int? productId)
        {
            if (productId == null)
            {
                return NotFound();
            }

            var product = await _context.Product.Include(e => e.Options!)
                .ThenInclude(x => x.OptionValues)
                .SingleOrDefaultAsync(e => e.Id == productId);
            var viewmodel = new VariantViewModel();

            if (product == null)
            {
                return NotFound();
            }
            else
            {
                viewmodel = new VariantViewModel()
                {
                    ProductId = product.Id,
                    Product = product.Name,
                };
                ViewBag.product = product;
            }

            return View(viewmodel);
        }

        // POST: Variants/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VariantViewModel viewModel, int productId, List<VariantValue> mySelect, IFormFile myimg)
        {
            if (ModelState.IsValid)
            {
                Variant variant = new Variant()
                {
                    SKU = viewModel.SKU,
                    ProductId = viewModel.ProductId,
                    Stock = viewModel.Stock,
                    Image = MyAppHelper.ImageToByteArray(myimg),
                    VariantValues = mySelect
                };
                _context.Add(variant);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                var product = await _context.Product.Include(e => e.Options!)

                    .ThenInclude(x => x.OptionValues)
                    .SingleOrDefaultAsync(e => e.Id == productId);
                ViewBag.product = product;
            }

            return View(viewModel);
        }

        // GET: Variants/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var variant = await _context.Variant.FindAsync(id);
            if (variant == null)
            {
                return NotFound();
            }
            ViewData["Product_Id"] = new SelectList(_context.Product, "Id", "Name", variant.ProductId);
            return View(variant);
        }

        // POST: Variants/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SKU,Image,Stock,Product_Id,Created_at,Modiftied_at")] Variant variant)
        {
            if (id != variant.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(variant);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VariantExists(variant.Id))
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
            ViewData["Product_Id"] = new SelectList(_context.Product, "Id", "Name", variant.ProductId);
            return View(variant);
        }

        // GET: Variants/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var variant = await _context.Variant
                .Include(v => v.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (variant == null)
            {
                return NotFound();
            }

            return View(variant);
        }

        // POST: Variants/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var variant = await _context.Variant.FindAsync(id);
            if (variant != null)
            {
                _context.Variant.Remove(variant);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VariantExists(int id)
        {
            return _context.Variant.Any(e => e.Id == id);
        }
    }
}
