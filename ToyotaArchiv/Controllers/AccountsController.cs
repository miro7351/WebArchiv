//#nullable disable
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PA.TOYOTA.DB;
using SessionHelper;
using ToyotaArchiv.Global;
using ToyotaArchiv.Infrastructure;

namespace ToyotaArchiv.Controllers
{
    public class AccountsController : Controller
    {

        private readonly ToyotaContext _context;
        private readonly ISessionService _sessionService;

        public AccountsController(ToyotaContext context, ISessionService sessionService)
        {
            _context = context;
            _sessionService = sessionService;   
        }


        /*
         * LoadData() sa spusti pri otvoreni stranky Index.cshtml, pozri datatableAccounts.js
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
                                select account).OrderBy(a => a.LoginId);

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

        //po otvoreni stranky z hl. menu: 'Ucty'
        public async Task<IActionResult> Index()
        {
           ( ViewBag.Login, ViewBag.Role ) =  _sessionService.ReadUserLoginAndRoleFromSession(HttpContext.Session);
            return View(await _context.Accounts.ToListAsync());
        }

        //Accounts: Index - po kliku na link 'Zmenit' napr.  '<a  href="/Accounts/Edit/27">Zmenit</a>
        [HttpGet]  //MH  link robi HtttpGet
        public async Task<IActionResult> Edit(int? id)
        {
            (ViewBag.Login, ViewBag.Role) = _sessionService.ReadUserLoginAndRoleFromSession(HttpContext.Session);
            var account = await _context.Accounts.FirstOrDefaultAsync(m => m.LoginId == id);

            //if (account == null)
            //{
            //    return NotFound();
            //}
            return View(account);
        }


        //Accounts: Index po kliku na link 'Novy ucet'
        public IActionResult Create()
        {
            (ViewBag.Login, ViewBag.Role) = _sessionService.ReadUserLoginAndRoleFromSession(HttpContext.Session);
            return View();
        }

        // POST: Accounts/Edit  submit
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LoginId,LoginName,LoginPassword,LoginRola,DbLogin,DbPassword,Aktivny")] Account account)
        {
            if (id != account.LoginId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(account);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    //string s = ex.Message;
                    if (!AccountExists(account.LoginId))
                    {
                        ViewBag.ErrorMessage = $"Chyba: účet '{account.LoginName}' neexistuje.";
                    }
                    else
                    {
                        ViewBag.ErrorMessage = $"Nastala chyba pri zmene účtu '{account.LoginName}'.";
                    }
                }
                catch
                {
                    ViewBag.ErrorMessage = $"Nastala chyba pri zmene účtu '{account.LoginName}'.";
                }

                return RedirectToAction(nameof(Index));
            }
            ViewBag.ErrorMessage = $"Nastala chyba pri zmene účtu '{account.LoginName}'.";
            return View(account);
        }


        /*public bool? Aktivny { get; set; }
         *  @Html.DisplayNameFor(m=>m.Aktivny)
            @Html.CheckBoxFor(m=>m.Aktivny.Value)   stale vrati null

          *    <label asp-for="Aktivny" class="control-label"></label>
               <input asp-for="Aktivny" class="form-control"  type="checkbox"/>  vytvori textbox kde sa neda nic zadat
         * 
         * -------------------
         * public bool Aktivny { get; set; } TOTO FUNGUJE OK!!!!
         * 
         *  @Html.DisplayNameFor(m=>m.Aktivny)
            @Html.CheckBoxFor(m=>m.Aktivny)   IsValid =true
         */

        //Create.cshtml po kliku na button 'Ulozit'
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Account account)  //MH 05.05.2022 zmena
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string? loginName = account.LoginName; //rola sa musi zadat, je to osetrene na klientovi 

                    if (loginName != null)
                    {
                       var myAccount =  _context.Accounts.FirstOrDefault(m => m.LoginName == loginName);
                        if(myAccount != null)//rolaDB uz existuje
                        {
                            ViewBag.ErrorMessage = $"Chyba: rola '{loginName}' už existuje, zadajte iný názov roly.";
                            return View(account);
                        }
                    }
                    //MH 06.05.2022: Ak Aktivny = false, do db sa zapise Aktivny=1!!!!!! Default value v db je 1.
                    //Ak Aktivny = false, do db sa zapise Aktivny=0!!!!!! Default value v db je 0.
                    _context.Add(account);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage ="Chyba: " + ex.Message;
                    return View(account);
                }
            }
            ViewBag.ErrorMessage = "Chyba pri spracovaní údajov.";
            return View(account);
        }


        public async Task<IActionResult> Delete(int? id)
        {
            (ViewBag.Login, ViewBag.Role) = _sessionService.ReadUserLoginAndRoleFromSession(HttpContext.Session);
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts.FirstOrDefaultAsync(m => m.LoginId == id);

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
            Account? account = await _context.Accounts.FindAsync(id);
            if (account != null)
            {
                _context.Accounts.Remove(account);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Delete));
        }

        private bool AccountExists(int id)
        {
            return _context.Accounts.Any(e => e.LoginId == id);
        }

    }
}
