#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PA.TOYOTA.DB;
using System.Linq.Dynamic.Core;

namespace ToyotaArchiv.Controllers
{
    //MH: april 2022
    public class ErrorsController : Controller
    {
        private readonly ToyotaContext _context;

        public ErrorsController(ToyotaContext context)
        {
            _context = context;
        }

        // GET: Errors
        public async Task<IActionResult> Index()
        {
            return View(await _context.Errors.ToListAsync());
        }

        //MH: funkcia sa spusti z klienta po otvoreni stranky, pomocou AJAXu
        [HttpPost]
        public IActionResult LoadData()
        {
            try
            {
                var draw = HttpContext.Request.Form["draw"].FirstOrDefault();
                // Skiping number of Rows count
                var start = Request.Form["start"].FirstOrDefault();
                // Paging Length 10,20
                var length = Request.Form["length"].FirstOrDefault();
                // Sort Column Name
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                // Sort Column Direction ( asc ,desc)
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                // Search Value from (Search box)
                var searchValue = Request.Form["search[value]"].FirstOrDefault();

                //Paging Size (10,20,50,100)
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;

                // Getting all Zakazka
                var chyby = (from chyba in _context.Errors
                               select chyba);

                //Sorting
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    chyby = chyby.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                //Search box sa nezobrazuje, pozri datatableErrors.js
                //if (!string.IsNullOrEmpty(searchValue))
                //{
                //    // tu vznika exception
                //    chyby = chyby.Where(  m => m.ErrorDate.ToString("dd.MM.yyyy HH:mm") == searchValue );
                //}

                //total number of rows count 
                recordsTotal = chyby.Count();
                //Paging 
                var data = chyby.Skip(skip).Take(pageSize).ToList();
                //Returning Json Data
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });

            }
            catch (Exception)
            {
                throw;
            }

        }

        /*
                // GET: Errors/Details/5
                public async Task<IActionResult> Details(int? id)
                {
                    if (id == null)
                    {
                        return NotFound();
                    }

                    var error = await _context.Errors
                        .FirstOrDefaultAsync(m => m.ErrorLogId == id);
                    if (error == null)
                    {
                        return NotFound();
                    }

                    return View(error);
                }

                // GET: Errors/Create
                public IActionResult Create()
                {
                    return View();
                }

                // POST: Errors/Create
                // To protect from overposting attacks, enable the specific properties you want to bind to.
                [HttpPost]
                [ValidateAntiForgeryToken]
                public async Task<IActionResult> Create([Bind("ErrorLogId,ErrorDate,ErrorMsg,ErrorNumber,ErrorProcedure,ErrorLine,User")] Error error)
                {
                    if (ModelState.IsValid)
                    {
                        _context.Add(error);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                    return View(error);
                }

                // GET: Errors/Edit/5
                public async Task<IActionResult> Edit(int? id)
                {
                    if (id == null)
                    {
                        return NotFound();
                    }

                    var error = await _context.Errors.FindAsync(id);
                    if (error == null)
                    {
                        return NotFound();
                    }
                    return View(error);
                }

                // POST: Errors/Edit/5
                // To protect from overposting attacks, enable the specific properties you want to bind to.
                // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
                [HttpPost]
                [ValidateAntiForgeryToken]
                public async Task<IActionResult> Edit(int id, [Bind("ErrorLogId,ErrorDate,ErrorMsg,ErrorNumber,ErrorProcedure,ErrorLine,User")] Error error)
                {
                    if (id != error.ErrorLogId)
                    {
                        return NotFound();
                    }

                    if (ModelState.IsValid)
                    {
                        try
                        {
                            _context.Update(error);
                            await _context.SaveChangesAsync();
                        }
                        catch (DbUpdateConcurrencyException)
                        {
                            if (!ErrorExists(error.ErrorLogId))
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
                    return View(error);
                }

                // GET: Errors/Delete/5
                public async Task<IActionResult> Delete(int? id)
                {
                    if (id == null)
                    {
                        return NotFound();
                    }

                    var error = await _context.Errors
                        .FirstOrDefaultAsync(m => m.ErrorLogId == id);
                    if (error == null)
                    {
                        return NotFound();
                    }

                    return View(error);
                }

                // POST: Errors/Delete/5
                [HttpPost, ActionName("Delete")]
                [ValidateAntiForgeryToken]
                public async Task<IActionResult> DeleteConfirmed(int id)
                {
                    var error = await _context.Errors.FindAsync(id);
                    _context.Errors.Remove(error);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }

                private bool ErrorExists(int id)
                {
                    return _context.Errors.Any(e => e.ErrorLogId == id);
                }
                */
    }
}
