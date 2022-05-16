using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

using PA.TOYOTA.DB;
using ToyotaArchiv.Infrastructure;

namespace ToyotaArchiv.Controllers
{
    //MH: maj 2022
    //Upraveny kod z AccountsController, autor: LV;

    /// <summary>
    /// Controller  pre logovanie a odhlasenie uzivatela;
    /// </summary>
    public class LoginController : Controller
    {

        private readonly ToyotaContext _context;
        private readonly ISessionService _sessionService;

        public LoginController(ToyotaContext context, ISessionService sessionService)
        {
            _context = context;
            _sessionService = sessionService;
        }


        #region ===LOGIN ===
        public IActionResult Login()
        {
            return View();
        }

        //po kliku na button submit 'Vstup'
        [HttpPost]
        public IActionResult Login(string userLogin, string password)
        {
            if (string.IsNullOrEmpty(userLogin) || string.IsNullOrEmpty(password))
            {
                ViewBag.ErrorMessage = "Zadajte login a heslo.";
                return View();
            }

            if (!AccountExists(userLogin))
            {
                ViewBag.ErrorMessage = $"Login '{userLogin}' neexistuje.";
                return View();
            }
            ClaimsIdentity? identity = null;
            bool isAuthenticated = false;
            Account? account = null;

            try
            {
                //var user = _context.Accounts.Where(f => f.LoginName == username && f.LoginPassword == password).FirstOrDefault();
                //var user = _context.Accounts.FirstOrDefault(f => f.LoginName == userLogin && f.LoginPassword == password && f.Aktivny == true);
                account = _context.Accounts.FirstOrDefault(f => f.LoginName == userLogin && f.LoginPassword == password);
                if (account == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid Email or Password");

                    ViewBag.ErrorMessage = $"Pre login '{userLogin}' je heslo neplatné.";
                    return View();
                }
                else
                {
                    if (!account.Aktivny)
                    {
                        ViewBag.ErrorMessage = $"Pre login '{userLogin}' účet je zablokovaný.";
                        return View();
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                ViewBag.ErrorMessage = "CHYBA pri spojení s databázou.";
                return View();
            }

            identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, account.LoginName),
                new Claim(ClaimTypes.Role, account.LoginRola)
            }, CookieAuthenticationDefaults.AuthenticationScheme);
            isAuthenticated = true;


            UserDetail userDetail = new UserDetail(login: account.LoginName, rola: account.LoginRola, dbLogin: account.DbLogin, dbPassword: account.DbPassword);
            _sessionService.WriteUserToSession(HttpContext.Session, userDetail);

            if (isAuthenticated && identity != null)
            {
                var principal = new ClaimsPrincipal(identity);
                var login = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                return RedirectToAction("Index", "ZakazkyJQ");
            }

            return View();
        }

        public IActionResult Logout()
        {
           // UserDetail user = new UserDetail();
            HttpContext.Session.Clear();
            //_sessionService.WriteUserToSession(HttpContext.Session, user);
            return View(nameof(Login));
        }


        private bool AccountExists(string login)
        {
            return _context.Accounts.Any(e => e.LoginName == login);
        }

        #endregion ===LOGIN ===
    }
}
