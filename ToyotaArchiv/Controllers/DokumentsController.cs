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
using System.Linq.Dynamic.Core;

namespace ToyotaArchiv.Controllers
{
    //MH: april 2022,  18.04.2022  NEJDE TO!!!!
    //pre zobrazenie zaznamov z db tab. Dokument.
    //Pouziva jQuery datatable pre zobrazenie udajov

    public class DokumentsController : Controller
    {
        private readonly ToyotaContext _context;

        public DokumentsController(ToyotaContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {

            ViewBag.Login = MHsessionService.ReadLoginFromSession(HttpContext.Session);
            ViewBag.Role = MHsessionService.ReadRoleFromSession(HttpContext.Session);

            var toyotaContext = _context.Dokuments;
            return View(await toyotaContext.ToListAsync());
        }

        #region ==old kod ===
        /*
        // GET: Dokuments
        public async Task<IActionResult> Index()
        {

            ViewBag.Login = MHsessionService.ReadLoginFromSession(HttpContext.Session);
            ViewBag.Role = MHsessionService.ReadRoleFromSession(HttpContext.Session);

            var toyotaContext = _context.Dokuments.Include(d => d.ZakazkaTgNavigation);
            var dokumenty =  await toyotaContext.ToListAsync();
            int pz = dokumenty.Count;   

            Dokument d1 = dokumenty[0]; //d1 obsahuje este kolekciu DokumentDetails, a Zakazka ZakazkaTgNavigation

            var d2 = (from d in dokumenty 
                     select new
                     {
                         DokumentID = d.DokumentId,
                         ZakazkaTG = d.ZakazkaTg,
                         NazovDokumentu = d.NazovDokumentu,
                         NazovSuboru = d.NazovSuboru,
                         DokumentPlatny = d.DokumentPlatny, 
                         Skupina = d.Skupina,
                         Vytvoril = d.Vytvoril, 
                         Vytvorene=d.Vytvorene, 
                         Zmenil = d.Zmenil, 
                         Zmenene=d.Zmenene, 
                         Poznamka=d.Poznamka
                     }).ToDynamicListAsync();   


            //return View(await toyotaContext.ToListAsync());toto zbehlo ked som to spustil pre povodnu stranku Index.cshtml
            return View(d2);
        }

        //MH: funkcia sa spusti z klienta po otvoreni stranky, pomocou AJAXu
        [HttpPost]
    public  IActionResult LoadData()
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
            // Search Value from (Search box)
            var searchValue = Request.Form["search[value]"].FirstOrDefault();

            //Paging Size (10,20,50,100)
            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;

            // Getting all Zakazka
            //var dokumenty = (from dokument in _context.Zakazkas
            //               select dokument);

            var dokumenty = _context.Dokuments.Include(d => d.ZakazkaTgNavigation);//.ToList();

            var d2 = (from d in dokumenty
                      select new
                      {
                          DokumentID = d.DokumentId,
                          ZakazkaTG = d.ZakazkaTg,
                          NazovDokumentu = d.NazovDokumentu,
                          NazovSuboru = d.NazovSuboru,
                          DokumentPlatny = d.DokumentPlatny,
                          Skupina = d.Skupina,
                          Vytvoril = d.Vytvoril,
                          Vytvorene = d.Vytvorene,
                          Zmenil = d.Zmenil,
                          Zmenene = d.Zmenene,
                          Poznamka = d.Poznamka
                      }).ToList();

            //Sorting
            //if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
            //{
            //    dokumenty = dokumenty.OrderBy(sortColumn + " " + sortColumnDirection);
            //}
            //Search
            //if (!string.IsNullOrEmpty(searchValue))
            //{
            //    //dokumenty = dokumenty.Where(m => m.ZakazkaTg == searchValue || m.Skupina.ToString() == searchValue );
            //    dokumenty = dokumenty.Where(m => m.ZakazkaTg == searchValue );
            //}

            //total number of rows count 
            recordsTotal = d2.Count;
            //Paging 
            //var data = dokumenty.Skip(skip).Take(pageSize).ToList();
            var data = d2.Skip(skip).Take(pageSize).ToList();
            //Returning Json Data
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });

        }
        catch (Exception)
        {
            throw;
        }

    }
     */
        #endregion ==old kod ===

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
                // Search Value from (Search box)
                var searchValue = Request.Form["search[value]"].FirstOrDefault();

                //Paging Size (10,20,50,100)
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;

                // Getting all Zakazka
                //var dokumenty = (from dokument in _context.Zakazkas
                //               select dokument);

                var dokumenty = (from dokument in _context.Dokuments    //.Include(d => d.ZakazkaTgNavigation);//.ToList();
                                 select dokument);


                //Sorting
                //if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                //{
                //    dokumenty = dokumenty.OrderBy(sortColumn + " " + sortColumnDirection);
                //}
                //Search
                if (!string.IsNullOrEmpty(searchValue))
                {
                    //dokumenty = dokumenty.Where(m => m.ZakazkaTg == searchValue || m.Skupina.ToString() == searchValue );
                    dokumenty = dokumenty.Where(m => m.ZakazkaTg == searchValue);
                }

                //total number of rows count 
                recordsTotal = dokumenty.Count();
                //Paging 
                var data = dokumenty.Skip(skip).Take(pageSize).ToList();
             
                //Returning Json Data
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });

            }
            catch (Exception)
            {
                throw;
            }

        }


        // GET: Dokuments/Details/5
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

        // GET: Dokuments/Create
        public IActionResult Create()
        {
            ViewData["ZakazkaTg"] = new SelectList(_context.Zakazkas, "ZakazkaTg", "ZakazkaTg");
            return View();
        }

        // POST: Dokuments/Create
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

        // GET: Dokuments/Edit/5
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

        // POST: Dokuments/Edit/5
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

        // GET: Dokuments/Delete/5
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

        // POST: Dokuments/Delete/5
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
