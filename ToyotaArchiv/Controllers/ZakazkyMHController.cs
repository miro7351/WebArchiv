using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PA.TOYOTA.DB;
using ToyotaArchiv.Models;

namespace ToyotaArchiv.Controllers
{
    public class ZakazkyMHController : Controller
    {

        private readonly ToyotaContext _context;

       
        public ZakazkyViewModel ZakazkyViewModel { get; set; }

        public ZakazkyMHController(ToyotaContext context)
        {
            _context = context;
            //po prihlaseni uzivatela do Session ulozit jeho parametre;
            //zo Session nacitat parametre prihlaseneho uzivatela, tu len simulujem udaje pre CurrentUser

            ZakazkyViewModel = new ZakazkyViewModel();
            ZakazkyViewModel.StringList.Add("test1");
            ZakazkyViewModel.StringList.Add("test2");
            ZakazkyViewModel.CurrentUser = new User() { Login = "mh", UserRole = Infrastructure.USER_ROLE.ADMIN };
            ZakazkyViewModel.DatumDo = DateTime.Now;
            ZakazkyViewModel.DatumOd = ZakazkyViewModel.DatumDo.Value.AddDays(-30);
        }

        //Po kliku na button 'Nacitat'
        public IActionResult ReadData(ZakazkyViewModel viewModel)//tu nam pride viewModel zo stranky, ale je prazdny obsahuje len DatumOd, DatumDo!!!!!!
        {
            //if (ModelState.IsValid)
            //{
                var dtOd = viewModel.DatumOd;
                var dtDo = viewModel.DatumDo;
                var currentUser = viewModel.CurrentUser;

                return RedirectToAction("Index");
            //}
            //return RedirectToAction("Index");
        }

        // GET: ZakazkyMHController
        public async Task<ActionResult> Index()
        {
            ZakazkyViewModel.Zakazky = await _context.Zakazkas.ToListAsync();
            return View(ZakazkyViewModel);
        }

        // GET: ZakazkyMHController/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var zakazka = await _context.Zakazkas
                .FirstOrDefaultAsync(m => m.ZakazkaTg == id);
            if (zakazka == null)
            {
                return NotFound();
            }

            return View(zakazka);
        }

        // GET: ZakazkyMHController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ZakazkyMHController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ZakazkaId,ZakazkaTg,ZakazkaTb,CisloProtokolu,Cws,Vin,Platna,Ukoncena,Vytvoril,Vytvorene,Zmenil,Zmenene,Poznamka")] Zakazka zakazka)
        {
            if (ModelState.IsValid)
            {
                _context.Add(zakazka);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(zakazka);
        }



        // GET: ZakazkyMHController/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var zakazka = await _context.Zakazkas.FindAsync(id);
            if (zakazka == null)
            {
                return NotFound();
            }
            return View(zakazka);
        }


        // POST: ZakazkyMHController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("ZakazkaId,ZakazkaTg,ZakazkaTb,CisloProtokolu,Cws,Vin,Platna,Ukoncena,Vytvoril,Vytvorene,Zmenil,Zmenene,Poznamka")] Zakazka zakazka)
        {
            if (id != zakazka.ZakazkaTg)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(zakazka);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ZakazkaExists(zakazka.ZakazkaTg))
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
            return View(zakazka);
        }

        // GET: ZakazkyMHController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ZakazkyMHController/Delete/5
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

        // POST: Zakazkas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var zakazka = await _context.Zakazkas.FindAsync(id);
            if( zakazka is not null)
            {
                _context.Zakazkas.Remove(zakazka);
                await _context.SaveChangesAsync();
            }
           
            return RedirectToAction(nameof(Index));
        }

        private bool ZakazkaExists(string id)
        {
            return _context.Zakazkas.Any(e => e.ZakazkaTg == id);
        }
    }
}
