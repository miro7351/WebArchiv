#nullable disable

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;  //z NUgetu

using PA.TOYOTA.DB;
using ToyotaArchiv.Infrastructure;

namespace ToyotaArchiv.Controllers
{
    public class ZakazkyJQController : Controller
    {
        private readonly ToyotaContext _context;

        public ZakazkyJQController(ToyotaContext context)
        {
            _context = context;
        }



        // GET: ZakazkyJQ
        public async Task<IActionResult> Index()
        {
            ViewBag.Login = MHsessionService.ReadLoginFromSession(HttpContext.Session);
            ViewBag.Role = MHsessionService.ReadRoleFromSession(HttpContext.Session);

            return View(await _context.Zakazkas.ToListAsync());
        }


        //MH: funkcia sa spusti z klienta po otvoreni stranky, pomocou AJAXu
        [HttpPost]
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
                var zakazky= (from zakazka in _context.Zakazkas
                                    select zakazka);

                //Sorting
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    zakazky = zakazky.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                //Search
                if (!string.IsNullOrEmpty(searchValue))
                {
                    zakazky = zakazky.Where(m => m.ZakazkaTb == searchValue ||
                                                m.ZakazkaTg == searchValue || 
                                                m.Vin == searchValue || 
                                                m.Cws == searchValue ||
                                                m.CisloProtokolu == searchValue ||
                                                m.Ukoncena == searchValue);
                }

                //total number of rows count 
                recordsTotal = zakazky.Count();
                //Paging 
                var data = zakazky.Skip(skip).Take(pageSize).ToList();
                //Returning Json Data
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });

            }
            catch (Exception)
            {
                throw;
            }

        }


        // GET: ZakazkyJQ/Details/5
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

        // GET: ZakazkyJQ/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ZakazkyJQ/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ZakazkaId,ZakazkaTg,ZakazkaTb,CisloProtokolu,Cws,Vin,Platna,Ukoncena,Vytvoril,Vytvorene,Zmenil,Zmenene,Poznamka")] Zakazka zakazka)
        {
            ViewBag.Login = MHsessionService.ReadLoginFromSession(HttpContext.Session);
            ViewBag.Role = MHsessionService.ReadRoleFromSession(HttpContext.Session);
            bool mv = ModelState.IsValid;
            
            //{if (ModelState.IsValid)
                zakazka.Vytvoril = ViewBag.Login;
                zakazka.Vytvorene= DateTime.Now;    
                _context.Add(zakazka);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            //}
            //return View(zakazka);
        }

        // GET: ZakazkyJQ/Edit/5  tu sa pride po kliku na ZakazkaTg, je zobrazena ako link
        //pozri: datatableZakazkyAdmin.js
        //volanie:  return '<a  href="/ZakazkyJQ/Edit2/?zakazkaTg=' + $.trim(row['zakazkaTg']) + '">' + data + '</a>';  IDE!!!!
        public async Task<IActionResult> Edit2(int ID)  //nazov parametra musi byt ID
        {
            if (ID == null)
            {
                return NotFound();
            }

            var zakazka = await _context.Zakazkas.FirstOrDefaultAsync(m => m.ZakazkaId == ID);
                
            if (zakazka == null)
            {
                return NotFound();
            }
            return View(zakazka);
        }


        //GET: ZakazkyJQ/Edit/5  tu sa pride po kliku na ZakazkaTg, je zobrazena ako link
        //volanie:  return '<a  href="/ZakazkyJQ/Edit2/zakazkaTg=' + $.trim(row['zakazkaTg']) + '">' + data + '</a>';  NEJDE!!
        //volanie:  return '<a  href="/ZakazkyJQ/Edit2/?zakazkaTg=' + $.trim(row['zakazkaTg']) + '">' + data + '</a>';  IDE!!!!
        public async Task<IActionResult> Edit(string zakazkaTg)
        {
            ViewBag.Login = MHsessionService.ReadLoginFromSession(HttpContext.Session);
            ViewBag.Role = MHsessionService.ReadRoleFromSession(HttpContext.Session);

            if (zakazkaTg == null)
            {
                return NotFound();
            }

            var zakazka = await _context.Zakazkas.FindAsync(zakazkaTg); /*MUSI MAT PRIMARY KEY!!! */
            if (zakazka == null)
            {
                return NotFound();
            }
            return View(zakazka);
        }

        /*
        // POST: ZakazkyJQ/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("ZakazkaId,ZakazkaTg,ZakazkaTb,CisloProtokolu,Cws,Vin,Platna,Ukoncena,Poznamka")] Zakazka zakazka)
        {
            //if (id != zakazka.ZakazkaTg)
            //{
            //    return NotFound();
            //}

            bool modelValid = ModelState.IsValid;

            //if (ModelState.IsValid)
            //{
                try
                {
                    _context.Update(zakazka);
                    await _context.SaveChangesAsync();//17.04.2022 Sql Exception: Cannot update Identity column ZakazkaID
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
            //}
            return View(zakazka);
        }
        */


        //17.04.2022 Sql Exception: Cannot update Identity column ZakazkaID
        //You can not update a column with IDENTITY property, you have to insert the record with new value and delete the one with the old value.

        // POST: ZakazkyJQ/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("ZakazkaId,ZakazkaTg,ZakazkaTb,CisloProtokolu,Cws,Vin,Platna,Ukoncena,Vytvoril,Vytvorene,Zmenil,Zmenene,Poznamka")] Zakazka zakazka)
        {
            //if (id != zakazka.ZakazkaId)
            //{
            //    return NotFound();
            //}

            var mp = ModelState.IsValid;

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
       

        // GET: ZakazkyJQ/Delete/5
        public async Task<IActionResult> Delete(int ID)
        {
            if (ID == null)
            {
                return NotFound();
            }

            var zakazka = await _context.Zakazkas
                .FirstOrDefaultAsync(m => m.ZakazkaId == ID);
            if (zakazka == null)
            {
                return NotFound();
            }

            return View(zakazka);
        }

        /*
         * 
         *  // GET: ZakazkyJQ/Delete/5
        public async Task<IActionResult> Delete(string zakazkaTg)
        {
            if (zakazkaTg == null)
            {
                return NotFound();
            }

            var zakazka = await _context.Zakazkas
                .FirstOrDefaultAsync(m => m.ZakazkaTg == zakazkaTg);
            if (zakazka == null)
            {
                return NotFound();
            }

            return View(zakazka);
        }
         */

        // POST: ZakazkyJQ/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string zakazkaTg)
        {
            var zakazka = await _context.Zakazkas.FindAsync(zakazkaTg);
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
