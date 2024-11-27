using Ecommerce_app.Areas.Admin.Models.ViewModels;
using Ecommerce_app.Data;
using Ecommerce_app.Helpers;
using Ecommerce_app.Models.Products;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce_app.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AttributesController : Controller
    {
        private readonly EcommerceAppContext _context;

        public AttributesController(EcommerceAppContext context)
        {
            _context = context;
        }

        #region Option
        public async Task<IActionResult> Index()
        {
            var options = await _context.Option.Include(e => e.OptionValues.OrderBy(v => v.Value)).ToListAsync();

            var viewModel = new List<OptionViewModel>();
            foreach(var item in options)
            {
                var viewModelItem = new OptionViewModel()
                {
                    Id = item.Id,
                    Type = item.Type,
                    Created_at = DateTime.Now,
                    Modiftied_at = DateTime.Now,
                    optionValuesVM = new List<OptionValueViewModel>()
                };
                foreach(var valueItem in item.OptionValues)
                {
                    var valueVM = new OptionValueViewModel()
                    {
                        Id = valueItem.Id,
                        Value = valueItem.Value,
                        Image = MyAppHelper.ViewImage(valueItem.Image!),
                        Created_at = valueItem.Created_at,
                        Modiftied_at = valueItem.Modiftied_at
                    };
                    viewModelItem.optionValuesVM.Add(valueVM);
                }
                viewModel.Add(viewModelItem);
            }

            return View(viewModel);
            //return View(await _context.Option.Include(e => e.OptionValues.OrderBy(v => v.Value)).ToListAsync());
        }

        public IActionResult AddOption()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOption([Bind("Id,Type")] Option option)
        {
            if (ModelState.IsValid)
            {
                _context.Add(option);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(option);
        }

        public async Task<IActionResult> EditOption(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var option = await _context.Option.FindAsync(id);
            if (option == null)
            {
                return NotFound();
            }
            return View(option);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditOption(int id, [Bind("id, Type")] Option option)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var exist = await _context.Option.FirstOrDefaultAsync(exist => exist.Id == option.Id);
                    if (exist != null)
                    {
                        _context.Update(option);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                catch (DbUpdateConcurrencyException) // 並行異常
                {
                    if (!OptionExists(option.Id))
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
            return View(option);
        }

        public async Task<IActionResult> DeleteOption(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var option = await _context.Option
                .FirstOrDefaultAsync(m => m.Id == id);
            if (option == null)
            {
                return NotFound();
            }

            return View(option);
        }

        [HttpPost, ActionName("DeleteOption")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var option = await _context.Option.FindAsync(id);
            if (option != null)
            {
                _context.Option.Remove(option);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OptionExists(int id)
        {
            return _context.Option.Any(e => e.Id == id);
        }
        #endregion

        #region OptionValue
        // GET: Attributes/Details/5/Value/Create
        public async Task<IActionResult> AddOptionValue(int? Option_Id)
        {
            if (Option_Id == null)
            {
                return NotFound();
            }

            var option = await _context.Option.FindAsync(Option_Id);
            if (option == null)
            {
                return NotFound();
            }
            else
            {
                ViewData["option_id"] = option.Id;
                ViewData["option_type"] = option.Type;
            }
            return View();
        }

        // POST: Attributes/Details/5/Value/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOptionValue(OptionValue optionValue)
        {
            if (ModelState.IsValid)
            {
                //optionValue.OptionId = Option_Id;
                _context.Add(optionValue);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            /*
            var option = await _context.Option
                .FirstOrDefaultAsync(m => m.Id == Option_Id);
            if (option == null)
            {
                return NotFound();
            }
            */
            return View(optionValue);
        }

        public async Task<IActionResult> EditOptionValue(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var optionValue = await _context.OptionValue.FindAsync(id);
            if (optionValue == null)
            {
                return NotFound();
            }
            return View(optionValue);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditOptionValue(int id, OptionValue optionValue, IFormFile myimg)
        {
            if (id != optionValue.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (myimg != null)
                    {
                        optionValue.Image = MyAppHelper.ImageToByteArray(myimg);
                    }
                    _context.Update(optionValue);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OptionValueExists(optionValue.Id))
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
            return View(optionValue);
        }

        public async Task<IActionResult> DeleteOptionValue(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var optionValue = await _context.OptionValue
                .FirstOrDefaultAsync(m => m.Id == id);
            if (optionValue == null)
            {
                return NotFound();
            }

            return View(optionValue);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteOptionValue(int id)
        {
            var optionValue = await _context.OptionValue.FindAsync(id);
            if (optionValue != null)
            {
                _context.OptionValue.Remove(optionValue);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OptionValueExists(int id)
        {
            return _context.OptionValue.Any(e => e.Id == id);
        }
        #endregion
    }
}
