#nullable disable
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PA.TOYOTA.DB;
using ToyotaArchiv.Global;
using ToyotaArchiv.Infrastructure;

namespace ToyotaArchiv.Controllers
{
    public class AccountsController : Controller
    {

        private readonly ToyotaContext _context;

        public AccountsController(ToyotaContext context)
        {
            _context = context;
        }

       
        /*LoadData() sa spusti pri otvoreni stranky Index2.cshtml, pozri datatableAccounts.js
       
         */
        [HttpPost]
        public IActionResult LoadData()
        {
            int skip;
            int pageSize;//pocet zaznamov na stranke

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
                pageSize = length != null ? Convert.ToInt32(length) : 0;
                skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;


                // Getting all accounts
                var accounts = (from account in _context.Accounts
                               select account).OrderBy(a=>a.LoginId);


                //Sorting
                //if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                //{
                //    accounts = accounts.OrderBy(sortColumn + " " + sortColumnDirection);
                //}

                //total number of rows count 
                recordsTotal = accounts.Count();
                //Paging 
                var data = accounts.Skip(skip).Take(pageSize).ToList(); 
              
                //Returning Json Data
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });

            }
            catch (Exception)
            {
                throw;
            }
        }//LoadData

        public async Task<IActionResult> Index()
        {
            return View(await _context.Accounts.ToListAsync());
        }

        public async Task<IActionResult> Index2()
        {
            return View(await _context.Accounts.ToListAsync());
        }


        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LoginName,LoginPassword,LoginRola")] Account account)
        {
            if (ModelState.IsValid)
            {
                _context.Add(account);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(account);
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts
                .FirstOrDefaultAsync(m => m.LoginId == id);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        // POST: Roles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var account = await _context.Accounts.FindAsync(id);
            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //private bool AccountExists(short id)
        //{
        //    return _context.Accounts.Any(e => e.LoginId == id);
        //}






        //public IActionResult Register()
        //{
        //    return View();
        //}

        //[HttpPost]
        //public IActionResult Register(string username, string password1, string password2, string userrole)
        //{
        //    return View();
        //}




        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) && string.IsNullOrEmpty(password))
            {
                return RedirectToAction("Login");
            }

            ClaimsIdentity? identity = null;
            bool isAuthenticated = false;


            var user = _context.Accounts.Where(f => f.LoginName == username && f.LoginPassword == password).FirstOrDefault();
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid Email or Password");
                return View();
            }

            identity = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.LoginName),
                    new Claim(ClaimTypes.Role, user.LoginRola)
                }, CookieAuthenticationDefaults.AuthenticationScheme);
            isAuthenticated = true;

            MHsessionService.WriteLoginToSession(HttpContext.Session, user.LoginName);
            MHsessionService.WriteRoleToSession(HttpContext.Session, user.LoginRola);
            AppData.SetCurrentUser(login: user.LoginName, role: user.LoginRola);


            //if (username == "admin" && password == "admin")
            //{
            //    identity = new ClaimsIdentity(new[]
            //    {
            //        new Claim(ClaimTypes.Name, username),
            //        new Claim(ClaimTypes.Role, "Admin")
            //    }, CookieAuthenticationDefaults.AuthenticationScheme);
            //    isAuthenticated = true;

            //}


            if (isAuthenticated && identity != null)
            {
                var principal = new ClaimsPrincipal(identity);
                var login = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                //return RedirectToAction("Index", "Home");
                return RedirectToAction("Index", "ZakazkyJQ");
            }

            return View();
        }

        public IActionResult Logout()
        {
            AppData.LogoutCurrentUser();
            return View(nameof(Login));
        }
    }
}
