using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PA.TOYOTA.DB;
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

        readonly ZakazkaTransformService _transService;

        public ZakazkaZOViewModel ZakazkaZOViewModel { get; set; }

        public ZakazkaMH2Controller(ToyotaContext context, ZakazkaTransformService transService)
        {
            _context = context;
            _transService = transService;
        }

        // GET: ZakazkaMH2Controller
        public async Task<ActionResult> Index()
        {
            var Zakazky = await _context.Zakazkas
                .Include(z => z.Dokuments)
               /* .ThenInclude(d => d.DokumentDetails)*/
                .OrderByDescending(z => z.Vytvorene)
                .ToListAsync();
            int pz = Zakazky.Count;
            if (pz > 0)
            {
                ZakazkaZOViewModel.MyZakazkaZO = _transService.ConvertZakazka_To_ZakazkaZO(Zakazky[0]);
                
            }
            return View(ZakazkaZOViewModel);
        }

        // GET: ZakazkaMH2Controller/Details/5
        public ActionResult Details(int id)
        {
            return View();
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
            return View();
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
            return View();
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
