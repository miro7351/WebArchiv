#nullable disable

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;  //z NUgetu

using PA.TOYOTA.DB;
using ToyotaArchiv.Infrastructure;
using ToyotaArchiv.Domain;


namespace ToyotaArchiv.Controllers
{
    //MH: april 2022
    //Zobrazenie, editovanie, mazanie zaznamov pre garancne opravy
    public class ZakazkyJQController : Controller
    {
        private readonly ToyotaContext _context;
        IZakazkaTransformService _transformService;

        public ZakazkyJQController(ToyotaContext context, IZakazkaTransformService transformService)
        {
            _context = context;
            _transformService = transformService;
        }

        // GET: ZakazkyJQ
        public async Task<IActionResult> Index()
        {
            ViewBag.Login = MHsessionService.ReadLoginFromSession(HttpContext.Session);
            ViewBag.Role = MHsessionService.ReadRoleFromSession(HttpContext.Session);

            //MH: TU SA NEMUSIA nacitat udaje, lebo po spusteni stranky sa spusti AJAX metoda na nacitanie udajov LoadData();

            //List<Zakazka> zakazky = await _context.Zakazkas.ToListAsync();
            //if(zakazky.Any())
            //{
            //    return View(zakazky);
            //}
            //else
            //{
            //    return NotFound();
            //}
            return View();


            //return NotFound();
            // vypise sa oznam: THis localhost page can't be found; 
            //No webpage was found for the web address: https://localhost:7036/ZakazkyJQ
            //HTTP ERROR 404
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
                var zakazky = (from zakazka in _context.Zakazkas
                               select zakazka);

                //Sorting
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    zakazky = zakazky.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                //Search
                if (!string.IsNullOrEmpty(searchValue))
                {
                    //zakazky = zakazky.Where(m => m.ZakazkaTb == searchValue ||
                    //                            m.ZakazkaTg == searchValue || 
                    //                            m.Vin == searchValue || 
                    //                            m.Cws == searchValue ||
                    //                            m.CisloProtokolu == searchValue ||
                    //                            m.Ukoncena == searchValue);
                    zakazky = zakazky.Where(m => m.ZakazkaTb.Contains(searchValue) ||
                                               m.ZakazkaTg.Contains(searchValue) ||
                                               m.Vin.Contains(searchValue) ||
                                               m.Cws.Contains(searchValue) ||
                                               m.CisloProtokolu.Contains(searchValue)
                                              );
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


        //GET: ZakazkyJQ/Edit/?zakazkaTg=ZakTG102  tu sa pride po kliku na link ZakazkaTg,  v tab. Garancne opravy
        //volanie:  return '<a  href="/ZakazkyJQ/Details/?zakazkaTg=' + $.trim(row['zakazkaTg']) + '">' + data + '</a>';  IDE!!!!
        public async Task<IActionResult> Details(string zakazkaTg)
        {
            ViewBag.Login = MHsessionService.ReadLoginFromSession(HttpContext.Session);
            ViewBag.Role = MHsessionService.ReadRoleFromSession(HttpContext.Session);

            if (zakazkaTg == null)
            {
                return NotFound();
            }

            //NACITANIE vsetkych Dokumentov a DokumentDetail-ov pre vybratu zakazku
            Zakazka zakazkaDB = await _context.Zakazkas
                .Where(m => m.ZakazkaTg == zakazkaTg)
               .Include(z => z.Dokuments)
               .ThenInclude(d => d.DokumentDetails)
               .OrderByDescending(z => z.Vytvorene)
               .FirstOrDefaultAsync();    //NACITANIE vsetkych Dokumentov a DokumentDetail-ov pre vybratu zakazku

            if (zakazkaDB == null)
                return RedirectToAction(nameof(Index));

            ZakazkaZO zakazkaZO = _transformService.ConvertZakazka_To_ZakazkaZO(ref zakazkaDB);
            return View(zakazkaZO);  //vrati Details.cshtml
        }

        // GET: ZakazkyJQ/Create
        public IActionResult Create()
        {
            return View();
        }

        /*
           // GET: ZakazkyJQ/Edit/5  tu sa pride po kliku na ZakazkaTg, je zobrazena ako link
           //pozri: datatableZakazkyAdmin.js
           //volanie:  return '<a  href="/ZakazkyJQ/Edit2/?zakazkaTg=' + $.trim(row['zakazkaTg']) + '">' + data + '</a>';  IDE!!!!
           public async Task<IActionResult> Edit2(int ID)  //nazov parametra musi byt ID
           {
               if (ID == null)
               {
                   return NotFound();
               }
               ViewBag.Login = MHsessionService.ReadLoginFromSession(HttpContext.Session);
               ViewBag.Role = MHsessionService.ReadRoleFromSession(HttpContext.Session);

               var zakazka = await _context.Zakazkas.FirstOrDefaultAsync(m => m.ZakazkaId == ID);

               if (zakazka == null)
               {
                   return NotFound();
               }
               return View(zakazka);
           }
           */

        //GET: ZakazkyJQ/Edit/?zakazkaTg=ZakTG102  tu sa pride po kliku na link ZakazkaTg,  v tab. Garancne opravy
        //volanie:  return '<a  href="/ZakazkyJQ/Edit/zakazkaTg=' + $.trim(row['zakazkaTg']) + '">' + data + '</a>';  NEJDE!!
        //volanie:  return '<a  href="/ZakazkyJQ/Edit/?zakazkaTg=' + $.trim(row['zakazkaTg']) + '">' + data + '</a>';  IDE!!!!
        public async Task<IActionResult> Edit(string zakazkaTg)
        {
            USER_ROLE userRole = MHsessionService.ReadRoleFromSession(HttpContext.Session);
            ViewBag.Login = MHsessionService.ReadLoginFromSession(HttpContext.Session);
            ViewBag.Role = MHsessionService.ReadRoleFromSession(HttpContext.Session);

            if (zakazkaTg == null)
            {
                return NotFound();
            }

            //var zakazka = await _context.Zakazkas.FindAsync(zakazkaTg); /* FindAsync MUSI MAT PRIMARY KEY!!! */
            //if (zakazka == null)
            //{
            //    return NotFound();
            //}
            // return View(zakazka);

            //NACITANIE vsetkych Dokumentov a DokumentDetail-ov pre vybratu zakazku
            Zakazka zakazkaDB = await _context.Zakazkas
                .Where(m => m.ZakazkaTg == zakazkaTg)
               .Include(z => z.Dokuments)
               .ThenInclude(d => d.DokumentDetails)
               .OrderByDescending(z => z.Vytvorene)
               .FirstOrDefaultAsync();    //NACITANIE vsetkych Dokumentov a DokumentDetail-ov pre vybratu zakazku

            if (zakazkaDB == null)
                return RedirectToAction(nameof(Index));

            ZakazkaZO zakazkaZO = _transformService.ConvertZakazka_To_ZakazkaZO(ref zakazkaDB);
            return View(zakazkaZO);  //vrati Edit.cshtml

            //if (( userRole== USER_ROLE.ADMIN) || (userRole== USER_ROLE.VEDUCI) )
            //    return View(zakazka); //moze sa aj editovat
            //else
            //    return View(zakazkaZO);  //len readonly 
        }

        /* link zo stranky: Edit
         * <td>@Html.ActionLink(@Model.ZakazkaTGdokument.NazovSuboru, "ShowImage", "ZakazkyJQ",
                new{ zakazkaTG=@Model.ZakazkaTg, subor=@Model.ZakazkaTGdokument.NazovSuboru, skupina=@Model.ZakazkaTGdokument.Skupina} )

        MH: OTVORI SA V OKNE BROWSERA KDE JE APLIKACIA!!! TAKZE ZRUSI APLIKACIU!!!!
         */

        //public async Task<FileStreamResult> ShowImage(string zakazkaTG, string subor, string skupina)
        public async Task<IActionResult> ShowImage(string zakazkaTG, string subor, string skupina)
        {
            if (string.IsNullOrEmpty(zakazkaTG) || string.IsNullOrEmpty(subor) || string.IsNullOrEmpty(skupina))
            {
                return null;
            }
            short mySkupina = 0;      //konvertuj skupina na short!!!
            short.TryParse(skupina, out mySkupina);
            if (mySkupina == 0)
                return null;

            Zakazka z1 = await (_context.Zakazkas
                      .Where(z => z.ZakazkaTg == zakazkaTG)
                      .Include(z => z.Dokuments)
                      .ThenInclude(d => d.DokumentDetails)
                      ).FirstOrDefaultAsync();
            if (z1 == null)
                return null;

            Dokument myDokument = z1.Dokuments.FirstOrDefault(d => d.Skupina == mySkupina);
            if (myDokument == null)
                return null;

            DokumentDetail d1 = myDokument.DokumentDetails?.FirstOrDefault(d => d.Skupina == mySkupina);
            if (d1 == null)
                return null;

            if (d1.DokumentContent == null)
                return null;

            Stream filecontent;
            string contentType = string.Empty;
            string fileName = myDokument.NazovSuboru.Trim();

            contentType = GetMIMEType(fileName);//contentType= jpg,...

            using (MemoryStream ms = new MemoryStream())
            {
                filecontent = new MemoryStream(d1.DokumentContent);
            }

            return File(filecontent, contentType);
        }

        /// <summary>
        /// Vrati hlavicku pre prenos suboru na klienta; autor LV;
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private string GetMIMEType(string fileName)
        {
            var provider = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider();

            string contentType;
            if (!provider.TryGetContentType(fileName, out contentType))
            {
                contentType = "application/octet-stream";
            }
            return contentType;
        }



        // POST: ZakazkyJQ/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("ZakazkaId,ZakazkaTg,ZakazkaTb,CisloProtokolu,Cws,Vin,Platna,Ukoncena,Vytvoril,Vytvorene,Zmenil,Zmenene,Poznamka")] Zakazka zakazka)
        {

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

        //Ak je prihlaseny ADMIN alebo VEDUCI, potom po kliku na link Vymazat v tab. "Garancne opravy" sa otvori formular Delete.
        //Ak vo formulari delete klikne uzivatel na button "Vymazat", potom sa prejde na DeleteConfirmed(string zakazkaTg).

        // GET: ZakazkyJQ/Delete/5  po kliku na Link Vymazat ak je zobrazeny zoznam Garancne opravy
        public async Task<IActionResult> Delete(int ID)
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
            return View(zakazka);//zobrazi sa Delete view
        }

        // POST: ZakazkyJQ/Delete/ZakTB1001
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


        /*
         *  public IFormFile? TBFile { get; set; } = null!;
         *  public IFormFile? TGFile { get; set; } = null!;
         *  
         *  BaseItem ZakazkaTBdokument  musi mat vsetky propery Nullable!!!! Potom je ModelState.IsValid = true!!
         */


        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("ZakazkaTg, ZakazkaTb, TBFile, TGFile,Vin,Cws,CisloProtokolu,Poznamka, Ukoncena, Vytvoril,Vytvorene,Zmenil,Zmenene")] ZakazkaZO zakazka)
        public async Task<IActionResult> Create([Bind("ZakazkaTg, ZakazkaTb, TBFile, TGFile,Vin,Cws,CisloProtokolu,Poznamka")] ZakazkaZO zakazka)
        {
            bool mv = ModelState.IsValid;

            if (ModelState.IsValid)
            {
                if (zakazka.TGFile != null)
                {
                    using (var ms = new MemoryStream())
                    {
                        zakazka.TGFile.CopyTo(ms);
                        zakazka.ZakazkaTGdokument.FileContent = ms.ToArray();
                        zakazka.ZakazkaTGdokument.NazovSuboru = zakazka.TGFile.FileName;
                        zakazka.ZakazkaTGdokument.NazovDokumentu = "ZakazkaTG";
                        zakazka.ZakazkaTGdokument.Skupina = (short)1;
                    }
                }

                if (zakazka.TBFile != null)
                {
                    using (var ms = new MemoryStream())
                    {
                        zakazka.TBFile.CopyTo(ms);
                        zakazka.ZakazkaTBdokument.FileContent = ms.ToArray();
                        zakazka.ZakazkaTBdokument.NazovSuboru = zakazka.TBFile.FileName;
                        zakazka.ZakazkaTBdokument.NazovDokumentu = "ZakazkaTB";
                        zakazka.ZakazkaTBdokument.Skupina = (short)2;
                    }
                }

                zakazka.Ukoncena = "N";

                Zakazka novaZakazka = _transformService.ConvertZakazkaZO_To_Zakazka(ref zakazka);
                _context.Add(novaZakazka);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(zakazka);
        }


        /*    NEJDE!!!
        //Tu sa prejde z Index.cshtml -> "Nova garancna oprava"
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("ZakazkaTg, ZakazkaTb, TBFile, TGFile,Vin,Cws,CisloProtokolu,Poznamka, Ukoncena, Vytvoril,Vytvorene,Zmenil,Zmenene")] ZakazkaZO zakazka)
        //public async Task<IActionResult> Create([Bind("ZakazkaTg, ZakazkaTb, TBFile, TGFile,Vin,Cws,CisloProtokolu,Poznamka")] ZakazkaZO zakazka)
        public async Task<IActionResult> Create([Bind("ZakazkaTg, ZakazkaTb, ZakazkaTGdokument.MyIFormFile, ZakazkaTBdokument.MyIFormFileVin,Cws,CisloProtokolu,Poznamka")] ZakazkaZO zakazka)
        {
            bool mv = ModelState.IsValid;

            if (ModelState.IsValid)
            {
                if (zakazka.ZakazkaTGdokument.MyIFormFile != null)
                {
                    using (var ms = new MemoryStream())
                    {
                        zakazka.ZakazkaTGdokument.MyIFormFile.CopyTo(ms);
                        zakazka.ZakazkaTGdokument.FileContent = ms.ToArray();
                        zakazka.ZakazkaTGdokument.NazovSuboru = zakazka.TGFile.FileName;
                        zakazka.ZakazkaTGdokument.NazovDokumentu = "ZakazkaTG";
                        zakazka.ZakazkaTGdokument.Skupina = (short)1;
                    }
                }

                if (zakazka.ZakazkaTBdokument.MyIFormFile != null)
                {
                    using (var ms = new MemoryStream())
                    {
                        zakazka.ZakazkaTBdokument.MyIFormFile.CopyTo(ms);
                        zakazka.ZakazkaTBdokument.FileContent = ms.ToArray();
                        zakazka.ZakazkaTBdokument.NazovSuboru = zakazka.TBFile.FileName;
                        zakazka.ZakazkaTBdokument.NazovDokumentu = "ZakazkaTB";
                        zakazka.ZakazkaTBdokument.Skupina = (short)2;
                    }
                }

                zakazka.Ukoncena = "N";

                Zakazka novaZakazka = _transformService.ConvertZakazkaZO_To_Zakazka( ref zakazka );
                _context.Add(novaZakazka);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(zakazka);
        }
    }
    */
    }

}
