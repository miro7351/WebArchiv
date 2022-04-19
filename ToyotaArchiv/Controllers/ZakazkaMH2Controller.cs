using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PA.TOYOTA.DB;
using ToyotaArchiv.Global;
using ToyotaArchiv.Infrastructure;
using ToyotaArchiv.Interfaces;
using ToyotaArchiv.Models;
using ToyotaArchiv.Services;

namespace ToyotaArchiv.Controllers
{
    /*
     * Vytvorit  Views\ZakazkaMH2\Index.cshtml
     */
    public class ZakazkaMH2Controller : Controller
    {

        private readonly ToyotaContext _context;

        readonly IZakazkaTransformService _transService;
        public User CurrentUser { get; set; }

        //pre testovanie ci sa da nastavovat z roznych view
        //!!!!  MUSI BYT static!!!  NEPOUZIT CACHE ?????
        public static int MyCounter { get; set; } = 0;

        public ZakazkaZOViewModel ZakazkaZOViewModel { get; set; }

        // !!!! ctor sa spusta vzdy ked sa prejde na ine View aj v ramci ZakazkaMH2Controller !!!!!
        public ZakazkaMH2Controller(ToyotaContext context, IZakazkaTransformService transService)
        {
            _context = context;
            _transService = transService;
            //zo Session nacitat parametre prihlaseneho uzivatela
            CurrentUser = new User() { Login = "mh", UserRole = USER_ROLE.ADMIN };

            ZakazkaZOViewModel = new ZakazkaZOViewModel() { CurrentUser = CurrentUser };    
        }

        // GET: ZakazkaMH2Controller, pozri Layout
        public async Task<ActionResult> Index()
        {
            //nacitanie parametrov zo session
            //Session hodnoty nastavene v HomeController: Index
            ViewBag.Name = HttpContext.Session.GetString(AppData.SessionName);
            ViewBag.Age = HttpContext.Session.GetInt32(AppData.SessionAge);
           // MyCounter++;
            ViewBag.MyCounter = MyCounter;  

            //ViewBag.Login = HttpContext.Session.GetString(AppData.SessionLogin);
            //ViewBag.Role = HttpContext.Session.GetString(AppData.SessionRole);

            ViewBag.Login = MHsessionService.ReadLoginFromSession(HttpContext.Session);
            ViewBag.Role = MHsessionService.ReadRoleFromSession(HttpContext.Session);

            ZakazkyViewModel zvm = new ZakazkyViewModel();
            var zakazky = await _context.Zakazkas
                .Include(z => z.Dokuments)
               /* .ThenInclude(d => d.DokumentDetails)*/
                .OrderByDescending(z => z.Vytvorene)
                .ToListAsync();
            int pz = zakazky.Count;
            if (pz > 0)
            {
                zvm.Zakazky = zakazky;
                zvm.DatumDo = DateTime.Now.AddHours(10);
                zvm.DatumOd = DateTime.Now.AddDays(-30); ;
            }

            return View(zvm);
        }

        //nakodovat: https://docs.microsoft.com/en-us/aspnet/core/mvc/views/tag-helpers/built-in/anchor-tag-helper?view=aspnetcore-6.0
        //Details.cshtml ...add new item... View empty
        // volanie zo Zakazkas Index.cshtml
        //  <a asp-action="Details" asp-controller="ZakazkaMH2"  asp-route-zakazkaTG="@item.ZakazkaTg">Detaily</a>
        // @Html.ActionLink(" Detail ", "Details", "ZakazkaMH2", new { zakazkaTg=item.ZakazkaTg, zakazkaTb=item.zakazkaTb})
        // GET: ZakazkaMH2Controller/Details/5
        public async Task<ActionResult> Details(string zakazkaTG)
        {
            //nacitanie parametrov zo session
            //Session hodnoty nastavene v HomeController: Index
            ViewBag.Name = HttpContext.Session.GetString(AppData.SessionName);
            ViewBag.Age = HttpContext.Session.GetInt32(AppData.SessionAge);
            //ViewBag.Login = HttpContext.Session.GetString(AppData.SessionLogin);
            //ViewBag.Role = HttpContext.Session.GetString(AppData.SessionRole);

            ViewBag.Login = MHsessionService.ReadLoginFromSession(HttpContext.Session);
            ViewBag.Role = MHsessionService.ReadRoleFromSession(HttpContext.Session);

            var zakazka = await _context.Zakazkas
                .Where(z=>z.ZakazkaTg == zakazkaTG)
               .Include(z => z.Dokuments)
               .FirstOrDefaultAsync();
            
            if(zakazka is not null )
            {
                ZakazkaZOViewModel.MyZakazkaZO = _transService.ConvertZakazka_To_ZakazkaZO(zakazka);
                return View(ZakazkaZOViewModel);
            }
            return RedirectToAction("Index");
           
        }
        //test pre 2 parametre v Html.ActionLink
        public async Task<ActionResult> Details2(string zakazkaTG, string zakazkaTB)
        {
            //nacitanie parametrov zo session
            //Session hodnoty nastavene v HomeController: Index
            ViewBag.Name = HttpContext.Session.GetString(AppData.SessionName);
            ViewBag.Age = HttpContext.Session.GetInt32(AppData.SessionAge);
            MyCounter++;
            ViewBag.MyCounter = MyCounter;

            //ViewBag.Login = HttpContext.Session.GetString(AppData.SessionLogin);
            //ViewBag.Role = HttpContext.Session.GetString(AppData.SessionRole);

            ViewBag.Login = MHsessionService.ReadLoginFromSession(HttpContext.Session);
            ViewBag.Role = MHsessionService.ReadRoleFromSession(HttpContext.Session);

            var zakazka = await _context.Zakazkas
                .Where(z => z.ZakazkaTg == zakazkaTG)
               .Include(z => z.Dokuments)
               .FirstOrDefaultAsync();

            if (zakazka is not null)
            {
                ZakazkaZOViewModel.MyZakazkaZO = _transService.ConvertZakazka_To_ZakazkaZO(zakazka);
                return View(ZakazkaZOViewModel);
            }
            return RedirectToAction("Index");

        }


        // GET: ZakazkaMH2Controller/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ZakazkaMH2Controller/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ZakazkaMH2Controller/Edit/5
        public ActionResult Edit(int id)
        {
            //return View();
            //return RedirectToAction("Index");
            return RedirectToAction(nameof(Index));
        }

        // POST: ZakazkaMH2Controller/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ZakazkaMH2Controller/Delete/5
        public ActionResult Delete(int id)
        {
            MyCounter++;
            //return View();  //vrati view Delete.cshtml
            return RedirectToAction(nameof(Index));
        }

        // POST: ZakazkaMH2Controller/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
