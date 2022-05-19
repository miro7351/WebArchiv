#nullable disable

using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Dynamic.Core;

using PA.TOYOTA.DB;
using ToyotaArchiv.Infrastructure;


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
        public IActionResult Index()//  public async Task<IActionResult> Index()
        {
            (ViewBag.Login, ViewBag.Role) = _sessionService.ReadUserLoginAndRoleFromSession(HttpContext.Session);
            return View();  //View(await _context.Logs1.ToListAsync());
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
                var colZakazkaTGSearchValue = Request.Form["columns[2][search][value]"].FirstOrDefault().ToString();
                var colOperaciaSearchValue = Request.Form["columns[3][search][value]"].FirstOrDefault().ToString();
                var colParameterSearchValue = Request.Form["columns[4][search][value]"].FirstOrDefault().ToString();
                var colPovodnaHodnotaSearchValue = Request.Form["columns[5][search][value]"].FirstOrDefault().ToString();
                var colNovaHodnotaSearchValue = Request.Form["columns[6][search][value]"].FirstOrDefault().ToString();


                var colUzivatelSearchValue = Request.Form["columns[7][search][value]"].FirstOrDefault().ToString();

                // Search Value from (Search box);   "filter": true, // pozri datatableLogy.js
                var searchValue = Request.Form["search[value]"].FirstOrDefault();

                //Paging Size (10,20,50,100)
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;

                // Getting all Zakazka
                var logs = (from log in _context.Logs
                               select log);

                //Sorting
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    logs = logs.OrderBy(sortColumn + " " + sortColumnDirection);
                }

                //MH-----------
                if (!string.IsNullOrEmpty(colDatumSearchValue))
                {
                    _ = DateTime.TryParse(colDatumSearchValue, new CultureInfo("sk-SK"), DateTimeStyles.None, out DateTime dt);

                    if (dt != DateTime.MinValue)
                        logs = logs.Where(m => m.Datum.Value.Year == dt.Year && 
                        m.Datum.Value.Month == dt.Month &&
                        m.Datum.Value.Day == dt.Day &&
                         m.Datum.Value.Hour == dt.Hour &&
                        m.Datum.Value.Minute == dt.Minute
                        );
                }
                if (!string.IsNullOrEmpty(colZakazkaTGSearchValue))
                {
                    logs = logs.Where(m => m.TgZakazka.Contains(colZakazkaTGSearchValue));
                }
                if (!string.IsNullOrEmpty(colOperaciaSearchValue))
                {
                    logs = logs.Where(m => m.Operacia.Contains(colOperaciaSearchValue));
                }
                if (!string.IsNullOrEmpty(colParameterSearchValue))
                {
                    logs = logs.Where(m => m.Parameter.Contains(colParameterSearchValue));
                }
                if (!string.IsNullOrEmpty(colPovodnaHodnotaSearchValue))
                {
                    logs = logs.Where(m => m.PovodnaHodnota.Contains(colPovodnaHodnotaSearchValue));
                }
                if (!string.IsNullOrEmpty(colNovaHodnotaSearchValue))
                {
                    logs = logs.Where(m => m.NovaHodnota.Contains(colNovaHodnotaSearchValue));
                }
                if (!string.IsNullOrEmpty(colUzivatelSearchValue))
                {
                    logs = logs.Where(m => m.Uzivatel.Contains(colUzivatelSearchValue));
                }

                //------------------------
                //Search podla  hodnoty v search boxe celkom vpravo hore nad tabulkou
                if (!string.IsNullOrEmpty(searchValue))
                {
                    bool dtReady = DateTime.TryParse(searchValue, new CultureInfo("sk-SK"), System.Globalization.DateTimeStyles.None, out DateTime dt);

                    if (dtReady && dt != DateTime.MinValue)
                    {
                        logs = logs.Where(m => m.Datum.Value.Year == dt.Year && 
                        m.Datum.Value.Month == dt.Month &&
                        m.Datum.Value.Day == dt.Day &&
                        m.Datum.Value.Hour == dt.Hour &&
                        m.Datum.Value.Minute == dt.Minute

                        );
                    }
                    else
                    {
                        logs = logs.Where(m => m.TgZakazka.Contains(searchValue) ||
                                         m.Operacia.Contains(searchValue) ||
                                         m.Parameter.Contains(searchValue) ||
                                         m.PovodnaHodnota.Contains(searchValue) ||
                                         m.NovaHodnota.Contains(searchValue) ||
                                         m.Uzivatel.Contains(searchValue)
                                         );
                    }
                }

                //total number of rows count 
                recordsTotal = logs.Count();
                //Paging 
                var data = logs.OrderByDescending(l=>l.Id).Skip(skip).Take(pageSize).ToList();
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
