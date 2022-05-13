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
        private readonly ISessionService _sessionService;
        public LogsController(ToyotaContext context,    ISessionService sessionService  )
        {
            _context = context;
            _sessionService = sessionService;
    }


        // GET: Logs
        public async Task<IActionResult> Index()
        {
            (ViewBag.Login, ViewBag.Role) = _sessionService.ReadUserLoginAndRoleFromSession(HttpContext.Session);
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
                if (!string.IsNullOrEmpty(colTableNameSearchValue))
                {
                    logy = logy.Where(m => m.TableName.Contains(colTableNameSearchValue));
                }
                if (!string.IsNullOrEmpty(colLogMessageSearchValue))
                {
                    logy = logy.Where(m => m.LogMessage.Contains(colLogMessageSearchValue));
                }
                if (!string.IsNullOrEmpty(colUserActionSearchValue))
                {
                    logy = logy.Where(m => m.UserAction.Contains(colUserActionSearchValue));
                }
                if (!string.IsNullOrEmpty(colUserNameSearchValue))
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
                var data = logy.OrderByDescending(l=>l.LogId).Skip(skip).Take(pageSize).ToList();
                //Returning Json Data
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });

            }
            catch (Exception)
            {
                throw;
            }

        }
    }
}
