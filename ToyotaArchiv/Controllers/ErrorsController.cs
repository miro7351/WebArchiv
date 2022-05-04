#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PA.TOYOTA.DB;
using System.Globalization;
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
        /*
         * When making a request to the server using server-side processing,
         * DataTables will send the following data in order to let the server know what data is required:
         * draw : int
         * start: int
         * length:int
         * search[value]: string  To be applied to all columns which have searchable=true
         */


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
                //vysklada napr: columns[0][name]  ak sa sortuje podla columnu s indexom=0
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                // Sort Column Direction ( asc ,desc)
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();

                //aby fungoval filter musi byt:     "filter": true,  pozri datatableErrors.js  !!!!
                var colDatumSearchValue = Request.Form["columns[1][search][value]"].FirstOrDefault();// ?? string.Empty;
                var colErrorMsgSearchValue = Request.Form["columns[2][search][value]"].FirstOrDefault();//.ToString() ?? string.Empty;

                var colErrorNumberSearchValue = Request.Form["columns[3][search][value]"].FirstOrDefault();//.ToString() ?? string.Empty;
                var colErrProcedureSearchValue = Request.Form["columns[4][search][value]"].FirstOrDefault();//.ToString() ?? string.Empty;

                var colErrorLineSearchValue = Request.Form["columns[5][search][value]"].FirstOrDefault();//.ToString() ?? string.Empty;
                var colUserSearchValue = Request.Form["columns[6][search][value]"].FirstOrDefault();//.ToString() ?? string.Empty;

                //int colErrorNumber = !string.IsNullOrEmpty(colErrorNumberSearchValue)  ? Convert.ToInt32(colErrorNumberSearchValue) : 0;
                //int colErrorLine = !string.IsNullOrEmpty(colErrorLineSearchValue)  ? Convert.ToInt32(colErrorLineSearchValue) : 0;

                _ = int.TryParse(colErrorNumberSearchValue, out int colErrorNumber);
                _ = int.TryParse(colErrorLineSearchValue, out int colErrorLine);


                // Search Value from (Search box)
                //Global search value. To be applied to all columns which have searchable=true
                var searchValue = Request.Form["search[value]"].FirstOrDefault();//MH: searchValue je zo Search textboxu nad tabulkou!!! 

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

                //MH-----------
                if (!string.IsNullOrEmpty(colDatumSearchValue))
                {
                    _ = DateTime.TryParse(colDatumSearchValue, new CultureInfo("sk-SK"), DateTimeStyles.None, out DateTime dt);

                    if (dt != DateTime.MinValue)
                        chyby = chyby.Where(m => m.ErrorDate.Year == dt.Year && m.ErrorDate.Month == dt.Month && m.ErrorDate.Day == dt.Day);
                }
                if (!string.IsNullOrEmpty(colErrorMsgSearchValue))
                {
                    chyby = chyby.Where(m => m.ErrorMsg.Contains(colErrorMsgSearchValue));
                }
                if (!string.IsNullOrEmpty(colErrProcedureSearchValue))
                {

                    chyby = chyby.Where(m => m.ErrorProcedure.Contains(colErrProcedureSearchValue));
                }
                if (!string.IsNullOrEmpty(colUserSearchValue))
                {

                    chyby = chyby.Where(m => m.User.Contains(colErrorMsgSearchValue));
                }
                if (colErrorNumber > 0)
                {
                    chyby = chyby.Where(m => m.ErrorNumber == colErrorNumber);
                }
                if (colErrorLine > 0)
                {
                    chyby = chyby.Where(m => m.ErrorLine == colErrorLine);
                }

                //------------------------
                //Search box sa zobrazuje, pozri datatableErrors.js
                if (!string.IsNullOrEmpty(searchValue))
                {
                    chyby = chyby.Where(m => m.ErrorMsg.Contains(searchValue) ||
                                             m.ErrorProcedure.Contains(searchValue) ||
                                             m.ErrorProcedure.Contains(searchValue) ||
                                             m.User.Contains(searchValue));
                }

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
