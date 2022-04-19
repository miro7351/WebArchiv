#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DataTable1.DB.Model;
using System.Linq.Dynamic.Core;  //z NUgetu

namespace DataTable1.Controllers
{
    public class DemoGridController : Controller
    {
        private readonly CustomerContext _context;

        public DemoGridController(CustomerContext context)
        {
            _context = context;
        }

        // GET: DemoGrid  MH: po kliku na polozku menu, pozri _Layout.cshtml
        public async Task<IActionResult> Index()
        {
            ViewBag.Role = "ADMIN";
            //ViewBag.Role = "READONLY";
            return View(await _context.CustomerTbs.ToListAsync());
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

                // Getting all Customer data
                var customerData = (from tempcustomer in _context.CustomerTbs
                                    select tempcustomer);

                //Sorting
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    customerData = customerData.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                //Search
                if (!string.IsNullOrEmpty(searchValue))
                {
                    customerData = customerData.Where(m => m.Name == searchValue || m.PhoneNo == searchValue || m.City == searchValue);
                }

                //total number of rows count 
                recordsTotal = customerData.Count();
                //Paging 
                var data = customerData.Skip(skip).Take(pageSize).ToList();
                //Returning Json Data
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });

            }
            catch (Exception)
            {
                throw;
            }

        }





        // GET: DemoGrid/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customerTb = await _context.CustomerTbs
                .FirstOrDefaultAsync(m => m.CustomerId == id);
            if (customerTb == null)
            {
                return NotFound();
            }

            return View(customerTb);
        }

        // GET: DemoGrid/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DemoGrid/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomerId,Name,Address,Country,City,PhoneNo")] CustomerTb customerTb)
        {
            if (ModelState.IsValid)
            {
                _context.Add(customerTb);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(customerTb);
        }

        /*
         * Podla roly zobrazit editovacie View, alebo len readonly view
         */ 

        // GET: DemoGrid/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customerTb = await _context.CustomerTbs.FindAsync(id);
            if (customerTb == null)
            {
                return NotFound();
            }
            return View(customerTb);
        }

        // POST: DemoGrid/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CustomerId,Name,Address,Country,City,PhoneNo")] CustomerTb customerTb)
        {
            if (id != customerTb.CustomerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customerTb);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerTbExists(customerTb.CustomerId))
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
            return View(customerTb);
        }

        // GET: DemoGrid/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customerTb = await _context.CustomerTbs
                .FirstOrDefaultAsync(m => m.CustomerId == id);
            if (customerTb == null)
            {
                return NotFound();
            }

            return View(customerTb);
        }


        /*
         * VOLANIE OK!!!!
         *    return '<a href="/DemoGrid/Delete/' + $.trim(row['customerId']) + '">' + 'Vymazat(' + row['customerId'] + ') </a>'
        
         * public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customerTb = await _context.CustomerTbs
                .FirstOrDefaultAsync(m => m.CustomerId == id);
            if (customerTb == null)
            {
                return NotFound();
            }

            return View(customerTb);
        }
         */

        // POST: DemoGrid/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customerTb = await _context.CustomerTbs.FindAsync(id);
            _context.CustomerTbs.Remove(customerTb);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerTbExists(int id)
        {
            return _context.CustomerTbs.Any(e => e.CustomerId == id);
        }
    }
}
