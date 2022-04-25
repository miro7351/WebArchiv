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

            //MH: 22.04.2022 ak neexistuju udaje, potom JQuery datatable vypise; Neexistuju ziadne zaznamy
           
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


        //Details Tu sa pride po kliku na link ZakazkaTg,  v tab. Garancne opravy
        //Pozri datatableZakazkyAdmin.js, datatableZakazkyVeduci.js, datatableZakazkyReadOnly.js
        //volanie:  return '<a  href="/ZakazkyJQ/Details/?zakazkaTg=' + $.trim(row['zakazkaTg']) + '">' + data + '</a>';  IDE!!!!

        public async Task<IActionResult> Details(string zakazkaTg)
        {
            USER_ROLE rola = USER_ROLE.READONLY;
            ViewBag.Login = MHsessionService.ReadLoginFromSession(HttpContext.Session);
            ViewBag.Role = rola = MHsessionService.ReadRoleFromSession(HttpContext.Session);

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

            //ZakazkaZO je ViewMOdel pre typ Zakazka
            ZakazkaZO zakazkaZO = _transformService.ConvertZakazka_To_ZakazkaZO(ref zakazkaDB);
            if (rola == USER_ROLE.READONLY)
            {
                return View(zakazkaZO);  //vrati Details.cshtml
            }
            else
            {
               return View(nameof(UpdateZakazka), zakazkaZO); //vrati UpdateZakazka.cshtml s viemodelom zakazkaZO
            }
        }


        // Index.cshtml po kliku na link ZakazkaTG
        //Uprava uz existujucej zakazky
        public IActionResult UpdateZakazka(string zakazkaTg)  //pouziva Views\ZakazkyJQ\UPdateZakazka.cshtml 
        {
            ZakazkaZO zakazkaZO = _transformService.VytvorPrazdnuZakazkuZO();

            return View(zakazkaZO);
        }


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

        /* link zo stranky: Details, alebo UpdateZakazka
         * <td>@Html.ActionLink(@Model.ZakazkaTGdokument.NazovSuboru, "ShowImage", "ZakazkyJQ",
                new{ zakazkaTG=@Model.ZakazkaTg, subor=@Model.ZakazkaTGdokument.NazovSuboru, skupina=@Model.ZakazkaTGdokument.Skupina} )

        MH: Ak  sa OTVORI  V OKNE BROWSERA KDE JE APLIKACIA TREBA KLINUT NA SIPKU PRE PRECHOD NA PREDCHADZAJUCU STRANU A TAM BUDE NASA APLIKACIA!!!!
         */
        //Tu sa pride z Details, alebo UpdateZakazka po kliku na link nazvu suboru;
        //public async Task<FileStreamResult> ShowImage(string zakazkaTG, string subor, string skupina)
        public async Task<IActionResult> ShowImage(string zakazkaTG, string subor, string skupina)
        {
            if (string.IsNullOrEmpty(zakazkaTG) || string.IsNullOrEmpty(subor) || string.IsNullOrEmpty(skupina))
            {
                return null;
            }
           
            short.TryParse(skupina, out short mySkupina);
            if (mySkupina == 0)
                return null;

            Zakazka z1 = await (_context.Zakazkas
                      .Where(z => z.ZakazkaTg == zakazkaTG)
                      .Include(z => z.Dokuments)
                      .ThenInclude(d => d.DokumentDetails)
                      ).FirstOrDefaultAsync();
            if (z1 == null)
                return null;

            //Dokument pre zadanu zakazkaTG a zadanu Skupinu
            Dokument myDokument = z1.Dokuments.FirstOrDefault(d => d.Skupina == mySkupina);
            if (myDokument == null)
                return null;
            //Detail pre myDokument pre zadanu Skupinu
            DokumentDetail d1 = myDokument.DokumentDetails?.FirstOrDefault(d => d.Skupina == mySkupina);
            if (d1 == null)
                return null;

            if (d1.DokumentContent == null)
                return null;
  
            string fileName = myDokument.NazovSuboru.Trim();
            
            //mimeType = pre jpeg "image/jpg", pre txt text/plain, pre pdf application/pdf, png image/png
            string mimeType = GetMIMEType(fileName);
            Stream filecontent;
            using (MemoryStream ms = new MemoryStream())
            {
                filecontent = new MemoryStream(d1.DokumentContent);
            }

            //TODO: nedat do appsettings.json polozku: UserOpenFile true/false??
            //true: po kliku na subor sa v browsero ukaze prijate-downloadnute subory na karte v pravom hornom rohu;
            //false:po kliku na subor sa  v okne browsera ukaze sa stiahnuty obrazok!!!!

            //na strane web klienta sa prijaty subor ulozi do adresara "Downloads", browser ukaze prijate-downloadnute subory na karte v pravom hornom rohu;
            //return File(filecontent, "application/octet-stream", fileName);

            //return File(filecontent, mimeType);

            //Subor na klientovi sa otvori v tom okne web browsera kde bezi klient-aplikacia, takze ju akoby zrusi!!!
            //Ak sa klikne na <- v okne browsera ukaze sa obrazovka nasej aplikacie, aplikacia dalej funguje!!!!
            //Ak bezi aplikacia a sa klikne na -> v okne browsera ukaze sa stiahnuty obrazok!!!!
            //Mozeme prechadzat aplikacia <-> posledny stiahnuty obrazok!!
            //Ak sa v aplikacii zmeni stranka, obrazok sa strati!!!!
            //Ak sa otvoreny obrazok (karta v okne browsera) zrusi, klik na x v karte, potom sa zrusi aj aplikacia!!!


            //MH: 22.04.2022 na strane web klienta sa prijaty subor ulozi do adresara "Downloads", browser ukaze prijate-downloadnute subory na karte v pravom hornom rohu;
            //Je tam volba Open file...!!!!!!!!!!
            //Po kliku na Open sa subor otvori v novom okne!!!!
            return File(filecontent, mimeType, fileName);
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

        /*
         * The [Bind] attribute is one way to protect against over-posting. 
         * You should only include properties in the [Bind] attribute that you want to change. 
         * 
         * The HttpPost attribute specifies that this Edit method can be invoked only for POST requests.
         */


        
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
            //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            
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


       


        // Index.cshtml po kliku na link "Nova garancna oprava";  novy postup  24.04.2022
        //Vytvorenie novej zakazky, Ppre vsetky roly okrem roly READONLY!
        // GET: ZakazkyJQ/Create
        public IActionResult NovaZakazka()  //pouziva Views\ZakazkyJQ\Create2.cshtml  NovaZakazka
        {
            ZakazkaZO zakazkaZO = _transformService.VytvorPrazdnuZakazkuZO();

            return View(zakazkaZO);
        }

        //NovaZakazka.cshtml klik na button"Ulozit", po zadani parametrov pre novu zakazku;
        //Nahratie suboru z klienta na server a ulozenie do DB
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( ZakazkaZO zakazkaZO)
        {
            bool mv = ModelState.IsValid;

            if (ModelState.IsValid)
            {
               
                if (zakazkaZO.ZakazkaTGdokument.DokFormFile != null)
                {
                    using (var ms = new MemoryStream())
                    {
                        zakazkaZO.ZakazkaTGdokument.DokFormFile.CopyTo(ms);
                        zakazkaZO.ZakazkaTGdokument.FileContent = ms.ToArray();
                        zakazkaZO.ZakazkaTGdokument.NazovSuboru = zakazkaZO.ZakazkaTGdokument.DokFormFile.FileName;
                        zakazkaZO.ZakazkaTGdokument.NazovDokumentu = "ZakazkaTG";
                       // zakazka.ZakazkaTGdokument.Poznamka = zakazka.TGFile.ContentType;//len na skusku!!!
                        zakazkaZO.ZakazkaTGdokument.Skupina = (short)1;
                    }
                }

                if (zakazkaZO.ZakazkaTBdokument?.DokFormFile != null)
                {
                    using (var ms = new MemoryStream())
                    {
                        zakazkaZO.ZakazkaTBdokument.DokFormFile.CopyTo(ms);
                        zakazkaZO.ZakazkaTBdokument.FileContent = ms.ToArray();
                        zakazkaZO.ZakazkaTBdokument.NazovSuboru = zakazkaZO.ZakazkaTBdokument.DokFormFile.FileName;
                        zakazkaZO.ZakazkaTBdokument.NazovDokumentu = "ZakazkaTB";
                        //zakazka.ZakazkaTBdokument.Poznamka = zakazka.TGFile.ContentType;//len na skusku!!!
                        zakazkaZO.ZakazkaTBdokument.Skupina = (short)2;
                    }
                }
               
                zakazkaZO.Ukoncena = "N";

                int pp = zakazkaZO.PovinneDokumenty.Count;
                int pr = zakazkaZO.Prilohy.Count;

                foreach(var d in zakazkaZO.PovinneDokumenty)
                {
                    using (var ms = new MemoryStream())
                    {
                        d.DokFormFile.CopyTo(ms);
                        d.FileContent = ms.ToArray();
                        d.NazovSuboru = d.DokFormFile.FileName;
                    }
                }

                foreach (var d in zakazkaZO.Prilohy)
                {
                    using (var ms = new MemoryStream())
                    {
                        d.DokFormFile.CopyTo(ms);
                        d.FileContent = ms.ToArray();
                        d.NazovSuboru = d.DokFormFile.FileName;
                    }
                }

                try
                {
                    Zakazka novaZakazka = _transformService.ConvertZakazkaZO_To_Zakazka(ref zakazkaZO);
                    _context.Add(novaZakazka);
                    int pocetUlozenych = await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch //ak nastane exception pri spracovani, napr. zada sa ZakazkaTG, ktore uz existuje v DB!
                {
                    //string msg = ex.Message;
                    //chybovy oznam sa zobrazi v hornej casti stranky
                    zakazkaZO.ErrorMessage = $"!! Nastala chyba pri uložení záznamu pre ZakazkaTg: {zakazkaZO.ZakazkaTg} !!";  //ex.Message;
                }
            }
            return View(zakazkaZO);
        }

        //UpdateZakazka.cshtml klik na button"Ulozit", po zadani parametrov pre zmenenu zakazku;
        //Nahratie suboru z klienta na server a ulozenie do DB
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateZakazka(ZakazkaZO zakazkaZO)
        {
            bool mv = ModelState.IsValid;

            if (ModelState.IsValid)
            {
                zakazkaZO.ErrorMessage = $"!! Update funkcionalita este nie je dokoncena pre ZakazkaTg: {zakazkaZO.ZakazkaTg} !!";
/*
                string zakazkaTg = zakazkaZO.ZakazkaTg;
                Zakazka starZakazkaDB = await _context.Zakazkas
                .Where(m => m.ZakazkaTg == zakazkaTg)
               .Include(z => z.Dokuments)
               .ThenInclude(d => d.DokumentDetails)
               .OrderByDescending(z => z.Vytvorene)
               .FirstOrDefaultAsync();    //NACITANIE vsetkych Dokumentov a DokumentDetail-ov pre vybratu zakazku
               
                //if (staraZakazka != null)
                //{
                //    _context.Zakazkas.Remove(staraZakazka); 
                //   int result=_context.SaveChanges();    
                //}

                
                if (zakazkaZO.ZakazkaTGdokument.DokFormFile != null)
                {
                    using (var ms = new MemoryStream())
                    {
                        zakazkaZO.ZakazkaTGdokument.DokFormFile.CopyTo(ms);
                        zakazkaZO.ZakazkaTGdokument.FileContent = ms.ToArray();
                        zakazkaZO.ZakazkaTGdokument.NazovSuboru = zakazkaZO.ZakazkaTGdokument.DokFormFile.FileName;
                        zakazkaZO.ZakazkaTGdokument.NazovDokumentu = "ZakazkaTG";
                        // zakazka.ZakazkaTGdokument.Poznamka = zakazka.TGFile.ContentType;//len na skusku!!!
                        zakazkaZO.ZakazkaTGdokument.Skupina = (short)1;
                    }
                }

                if (zakazkaZO.ZakazkaTBdokument?.DokFormFile != null)
                {
                    using (var ms = new MemoryStream())
                    {
                        zakazkaZO.ZakazkaTBdokument.DokFormFile.CopyTo(ms);
                        zakazkaZO.ZakazkaTBdokument.FileContent = ms.ToArray();
                        zakazkaZO.ZakazkaTBdokument.NazovSuboru = zakazkaZO.ZakazkaTBdokument.DokFormFile.FileName;
                        zakazkaZO.ZakazkaTBdokument.NazovDokumentu = "ZakazkaTB";
                        //zakazka.ZakazkaTBdokument.Poznamka = zakazka.TGFile.ContentType;//len na skusku!!!
                        zakazkaZO.ZakazkaTBdokument.Skupina = (short)2;
                    }
                }

                zakazkaZO.Ukoncena = "N";

                int pp = zakazkaZO.PovinneDokumenty.Count;
                int pr = zakazkaZO.Prilohy.Count;

                foreach (var d in zakazkaZO.PovinneDokumenty)
                {
                    using (var ms = new MemoryStream())
                    {
                        d.DokFormFile.CopyTo(ms);
                        d.FileContent = ms.ToArray();
                        d.NazovSuboru = d.DokFormFile.FileName;
                    }
                }

                foreach (var d in zakazkaZO.Prilohy)
                {
                    using (var ms = new MemoryStream())
                    {
                        d.DokFormFile.CopyTo(ms);
                        d.FileContent = ms.ToArray();
                        d.NazovSuboru = d.DokFormFile.FileName;
                    }
                }

                try
                {
                    Zakazka novaZakazka = _transformService.ConvertZakazkaZO_To_Zakazka(ref zakazkaZO);
                    _context.Add(novaZakazka);
                    int pocetUlozenych = await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch //ak nastane exception pri spracovani, napr. zada sa ZakazkaTG, ktore uz existuje v DB!
                {
                    //string msg = ex.Message;
                    //chybovy oznam sa zobrazi v hornej casti stranky
                    zakazkaZO.ErrorMessage = $"!! Nastala chyba pri uložení záznamu pre ZakazkaTg: {zakazkaZO.ZakazkaTg} !!";  //ex.Message;
                }
                */
            }
            return View(zakazkaZO);
        }
    }

}
