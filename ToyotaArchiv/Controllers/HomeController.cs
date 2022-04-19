using Microsoft.AspNetCore.Mvc;
using PA.TOYOTA.DB;
using System.Diagnostics;
using ToyotaArchiv.Global;
using ToyotaArchiv.Infrastructure;
using ToyotaArchiv.Models;

namespace ToyotaArchiv.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ToyotaContext _context;

        public HomeController(ILogger<HomeController> logger, ToyotaContext context)
        {
            _logger = logger;
            _context = context; 
        }

        public IActionResult Index()
        {

#if DEBUG      
            ViewData["ConnString"] = _context.ConnectionString;
#else
            ViewData["ConnString"]=string.Empty;
#endif
            //HttpContext.Session is available after session state is configured.
            //testovanie session: nastavenie pociatocnych hodnot
            MHsessionService.WriteLoginToSession(HttpContext.Session, "MH");
            //MHsessionService.WriteRoleToSession(HttpContext.Session, USER_ROLE.ADMIN);

            MHsessionService.WriteRoleToSession(HttpContext.Session, USER_ROLE.VEDUCI);
            //MHsessionService.WriteRoleToSession(HttpContext.Session, USER_ROLE.USER1);
            //MHsessionService.WriteRoleToSession(HttpContext.Session, USER_ROLE.READONLY);
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}