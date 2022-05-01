#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

using PA.TOYOTA.DB;
using ToyotaArchiv.Infrastructure;
using System.Globalization;

namespace ToyotaArchiv.Controllers
{
    //MH: april 2022
    public class LogsController : Controller
    {
        private readonly ToyotaContext _context;

        public LogsController(ToyotaContext context)
        {
            _context = context;
        }


        // GET: Logs
        public async Task<IActionResult> Index()
        {
            ViewBag.Login = MHsessionService.ReadLoginFromSession(HttpContext.Session);
            ViewBag.Role = MHsessionService.ReadRoleFromSession(HttpContext.Session);
            return View(await _context.Logs.ToListAsync());
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


                //aby fungoval filter musi byt:     "filter": true,  pozri datatableLogy.js  !!!!
                var colDatumSearchValue = Request.Form["columns[1][search][value]"].FirstOrDefault() ?? string.Empty;
                var colTableNameSearchValue = Request.Form["columns[2][search][value]"].FirstOrDefault().ToString();
                var colLogMessageSearchValue = Request.Form["columns[3][search][value]"].FirstOrDefault().ToString();
                var colUserActionSearchValue = Request.Form["columns[4][search][value]"].FirstOrDefault().ToString();
                var colUserNameSearchValue = Request.Form["columns[5][search][value]"].FirstOrDefault().ToString();

                // Search Value from (Search box);   "filter": true, // pozri datatableLogy.js
                var searchValue = Request.Form["search[value]"].FirstOrDefault();

                //Paging Size (10,20,50,100)
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;

                // Getting all Zakazka
                var logy = (from log in _context.Logs
                               select log);

                //Sorting
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    logy = logy.OrderBy(sortColumn + " " + sortColumnDirection);
                }

                //MH-----------
                if (!string.IsNullOrEmpty(colDatumSearchValue))
                {
                    _ = DateTime.TryParse(colDatumSearchValue, new CultureInfo("sk-SK"), DateTimeStyles.None, out DateTime dt);

                    if (dt != DateTime.MinValue)
                        logy = logy.Where(m => m.LogDate.Year == dt.Year && m.LogDate.Month == dt.Month && m.LogDate.Day == dt.Day);
                }
                else if (!string.IsNullOrEmpty(colTableNameSearchValue))
                {
                    logy = logy.Where(m => m.TableName.Contains(colTableNameSearchValue));
                }
                else if (!string.IsNullOrEmpty(colLogMessageSearchValue))
                {
                    logy = logy.Where(m => m.LogMessage.Contains(colLogMessageSearchValue));
                }
                else if (!string.IsNullOrEmpty(colUserActionSearchValue))
                {
                    logy = logy.Where(m => m.UserAction.Contains(colUserActionSearchValue));
                }
                else if (!string.IsNullOrEmpty(colUserNameSearchValue))
                {
                    logy = logy.Where(m => m.UserName.Contains(colUserNameSearchValue));
                }

                //------------------------


                //Search podla  hodnoty v search boxe celkom vpravo hore nad tabulkou
                if (!string.IsNullOrEmpty(searchValue))
                {
                    logy = logy.Where(m=> m.TableName.Contains(searchValue) ||
                                     m.LogMessage.Contains(searchValue) ||
                                     m.UserAction.Contains(searchValue) ||
                                     m.UserName.Contains(searchValue));
                }

                //total number of rows count 
                recordsTotal = logy.Count();
                //Paging 
                var data = logy.Skip(skip).Take(pageSize).ToList();
                //Returning Json Data
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });

            }
            catch (Exception)
            {
                throw;
            }

        }


       

        /*
        // GET: Logs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var log = await _context.Logs
                .FirstOrDefaultAsync(m => m.LogId == id);
            if (log == null)
            {
                return NotFound();
            }

            return View(log);
        }

        // GET: Logs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Logs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LogId,LogDate,TableName,LogMessage,UserAction,UserName")] Log log)
        {
            if (ModelState.IsValid)
            {
                _context.Add(log);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(log);
        }

        // GET: Logs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var log = await _context.Logs.FindAsync(id);
            if (log == null)
            {
                return NotFound();
            }
            return View(log);
        }

        // POST: Logs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LogId,LogDate,TableName,LogMessage,UserAction,UserName")] Log log)
        {
            if (id != log.LogId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(log);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LogExists(log.LogId))
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
            return View(log);
        }

        // GET: Logs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var log = await _context.Logs
                .FirstOrDefaultAsync(m => m.LogId == id);
            if (log == null)
            {
                return NotFound();
            }

            return View(log);
        }

        // POST: Logs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var log = await _context.Logs.FindAsync(id);
            _context.Logs.Remove(log);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LogExists(int id)
        {
            return _context.Logs.Any(e => e.LogId == id);
        }
        */
    }
}
