#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PA.TOYOTA.DB;

namespace ToyotaArchiv.Controllers
{
    public class ZakazkasController : Controller
    {
        private readonly ToyotaContext _context;

        public ZakazkasController(ToyotaContext context)
        {
            _context = context;
        }

        // GET: Zakazkas
        public async Task<IActionResult> Index()
        {
            return View(await _context.Zakazkas.ToListAsync());
        }

        // GET: Zakazkas/Details/5
        public async Task<IActionResult> Details(string id)
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

        // GET: Zakazkas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Zakazkas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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

        // GET: Zakazkas/Edit/5
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

        // POST: Zakazkas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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

        // GET: Zakazkas/Delete/5
        public async Task<IActionResult> Delete(string id)
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

        // POST: Zakazkas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var zakazka = await _context.Zakazkas.FindAsync(id);
            _context.Zakazkas.Remove(zakazka);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ZakazkaExists(string id)
        {
            return _context.Zakazkas.Any(e => e.ZakazkaTg == id);
        }
    }
}
