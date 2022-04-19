#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using PA.TOYOTA.DB;
using ToyotaArchiv.Infrastructure;

namespace ToyotaArchiv.Controllers
{
    //MH: 18.04.2022  pouzivaju sa view vygenerovane VS pre entitu Dokument
    public class Dokuments2Controller : Controller
    {
        private readonly ToyotaContext _context;

        public Dokuments2Controller(ToyotaContext context)
        {
            _context = context;
        }

        // GET: Dokuments2
        public async Task<IActionResult> Index()
        {
            ViewBag.Login = MHsessionService.ReadLoginFromSession(HttpContext.Session);
            ViewBag.Role  = MHsessionService.ReadRoleFromSession(HttpContext.Session);
            //OK toto vygeneruje VS
            //zobrazuje aj ZakazkaTgNavigation.ZakazkaTg
            var dokumenty = _context.Dokuments.Include(d => d.ZakazkaTgNavigation);
            return View(await dokumenty.ToListAsync());

            //MH skuska, ide ale nezobrazi ZakazkaTg. Index.cshtml neobsahuje stlpec s nazvom ZakazkaTg
            //var dokumenty = _context.Dokuments;
            //return View(await dokumenty.ToListAsync());
        }

        // GET: Dokuments2/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dokument = await _context.Dokuments
                .Include(d => d.ZakazkaTgNavigation)
                .FirstOrDefaultAsync(m => m.DokumentId == id);
            if (dokument == null)
            {
                return NotFound();
            }

            return View(dokument);
        }

        // GET: Dokuments2/Create
        public IActionResult Create()
        {
            ViewData["ZakazkaTg"] = new SelectList(_context.Zakazkas, "ZakazkaTg", "ZakazkaTg");
            return View();
        }

        // POST: Dokuments2/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DokumentId,ZakazkaTg,NazovDokumentu,NazovSuboru,DokumentPlatny,Skupina,Vytvoril,Vytvorene,Zmenil,Zmenene,Poznamka")] Dokument dokument)
        {
            if (ModelState.IsValid)
            {
                _context.Add(dokument);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ZakazkaTg"] = new SelectList(_context.Zakazkas, "ZakazkaTg", "ZakazkaTg", dokument.ZakazkaTg);
            return View(dokument);
        }

        // GET: Dokuments2/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dokument = await _context.Dokuments.FindAsync(id);
            if (dokument == null)
            {
                return NotFound();
            }
            ViewData["ZakazkaTg"] = new SelectList(_context.Zakazkas, "ZakazkaTg", "ZakazkaTg", dokument.ZakazkaTg);
            return View(dokument);
        }

        // POST: Dokuments2/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DokumentId,ZakazkaTg,NazovDokumentu,NazovSuboru,DokumentPlatny,Skupina,Vytvoril,Vytvorene,Zmenil,Zmenene,Poznamka")] Dokument dokument)
        {
            if (id != dokument.DokumentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dokument);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DokumentExists(dokument.DokumentId))
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
            ViewData["ZakazkaTg"] = new SelectList(_context.Zakazkas, "ZakazkaTg", "ZakazkaTg", dokument.ZakazkaTg);
            return View(dokument);
        }

        // GET: Dokuments2/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dokument = await _context.Dokuments
                .Include(d => d.ZakazkaTgNavigation)
                .FirstOrDefaultAsync(m => m.DokumentId == id);
            if (dokument == null)
            {
                return NotFound();
            }

            return View(dokument);
        }

        // POST: Dokuments2/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dokument = await _context.Dokuments.FindAsync(id);
            _context.Dokuments.Remove(dokument);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DokumentExists(int id)
        {
            return _context.Dokuments.Any(e => e.DokumentId == id);
        }
    }
}
