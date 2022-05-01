#nullable disable

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;  //z NUgetu

using PA.TOYOTA.DB;
using ToyotaArchiv.Infrastructure;
using ToyotaArchiv.Domain;
using System.Globalization;

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
            ViewBag.Role = MHsessionService.ReadRoleFromSession(HttpContext.Session).ToString();

            //MH: TU SA NEMUSIA nacitat udaje, lebo po spusteni stranky sa spusti AJAX metoda na nacitanie udajov LoadData()
            //a ta si nacita zadany pocet zoznamov;

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

            //MH: 22.04.2022 ak neexistuju udaje, potom JQuery datatable vypise: "Nie sú k dispozícii žiadne data"
           
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

                var datumSearchValue = Request.Form["columns[1][search][value]"].FirstOrDefault() ?? string.Empty;
                var zakazkaTgSearchValue  = Request.Form["columns[2][search][value]"].FirstOrDefault() ?? string.Empty;
                var zakazkaTbSearchValue = Request.Form["columns[3][search][value]"].FirstOrDefault() ?? string.Empty;
                var vinSearchValue = Request.Form["columns[4][search][value]"].FirstOrDefault() ?? string.Empty;

                var cwsSearchValue = Request.Form["columns[5][search][value]"].FirstOrDefault() ?? string.Empty;
                var cisloProtSearchValue = Request.Form["columns[6][search][value]"].FirstOrDefault() ?? string.Empty;
                var ukoncenaSearchValue = Request.Form["columns[7][search][value]"].FirstOrDefault() ?? string.Empty;

                // Getting all Zakazka
                var zakazky = (from zakazka in _context.Zakazkas
                               select zakazka);

                //Sorting
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    zakazky = zakazky.OrderBy(sortColumn + " " + sortColumnDirection);
                }

                if (!string.IsNullOrEmpty(datumSearchValue))
                {
                    //zakazky = zakazky.Where(m => m.Vytvorene.Value.ToString("dd.MM.yyyy HH:mm").Contains(datumSearchValue));toto nejde!!
                    
                       _ = DateTime.TryParse(datumSearchValue, new CultureInfo("sk-SK"), System.Globalization.DateTimeStyles.None, out DateTime dt);

                    if(dt != DateTime.MinValue ) 
                         zakazky = zakazky.Where(m =>  m.Vytvorene.Value.Year==dt.Year && m.Vytvorene.Value.Month==dt.Month && m.Vytvorene.Value.Day == dt.Day);
                }
             
                if (!string.IsNullOrEmpty(zakazkaTgSearchValue))
                {
                    zakazky = zakazky.Where(m => m.ZakazkaTg.Contains(zakazkaTgSearchValue));
                }
                else if (!string.IsNullOrEmpty(zakazkaTbSearchValue))
                {
                    zakazky = zakazky.Where(m => m.ZakazkaTb.Contains(zakazkaTbSearchValue));
                }

                else if (!string.IsNullOrEmpty(vinSearchValue))
                {
                    zakazky = zakazky.Where(m => m.Vin.Contains(vinSearchValue));
                }
                else if (!string.IsNullOrEmpty(cwsSearchValue))
                {
                    zakazky = zakazky.Where(m => m.Cws.Contains(cwsSearchValue));
                }
                else if (!string.IsNullOrEmpty(cisloProtSearchValue))
                {
                    zakazky = zakazky.Where(m => m.CisloProtokolu.Contains(cisloProtSearchValue));
                }
                else if (!string.IsNullOrEmpty(ukoncenaSearchValue))
                {
                    zakazky = zakazky.Where(m => m.Ukoncena.Contains(ukoncenaSearchValue));
                }
                //--------------------
                //Search podla search okna celkom hore napravo
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
                                               m.CisloProtokolu.Contains(searchValue) ||
                                               m.Ukoncena.Contains( searchValue)
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
            // USER_ROLE rola = USER_ROLE.READONLY;
            string rola = "READONLY";
            ViewBag.Login = MHsessionService.ReadLoginFromSession(HttpContext.Session);
            ViewBag.Role = rola = MHsessionService.ReadRoleFromSession(HttpContext.Session).ToString();

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

            //ZakazkaZO je ViewModel pre instanciu  typu Zakazka
            ZakazkaZO zakazkaZO = _transformService.ConvertZakazka_To_ZakazkaZO(ref zakazkaDB);
            if (rola == "READONLY")
            {
                return View(zakazkaZO);  //vrati Details.cshtml
            }
            else
            {
               return View(nameof(UpdateZakazka), zakazkaZO); //vrati UpdateZakazka.cshtml s viemodelom zakazkaZO
            }
        }


        
        //Uprava uz existujucej zakazky
        public IActionResult UpdateZakazka(string zakazkaTg)  //pouziva Views\ZakazkyJQ\UPdateZakazka.cshtml 
        {
            short pocetPriloh = 1;
            ZakazkaZO zakazkaZO = _transformService.VytvorPrazdnuZakazkuZO(pocetPriloh: pocetPriloh);

            return View(zakazkaZO);
        }


        /*
         * UpdateZakazka.cshtml  klik na link Vymazat
             @Html.ActionLink("Vymazať", "DeleteDokument", "ZakazkyJQ",
                                    new{ zakazkaTG=@Model?.ZakazkaTg?.Trim(), skupina=@Model?.ZakazkaTGdokument?.Skupina} )
         * 
         */
        /// <summary>
        /// Odstrani dokument z _context.Dokuments, ulozi zmeny, nacita udaje pre zakazku, 
        /// transformuje ju do typu ZakazkaZO a zobrazi instanciu ZakazkaZO
        /// </summary>
        /// <param name="zakazkaTG"></param>
        /// <param name="subor"></param>
        /// <param name="skupina"></param>
        /// <returns></returns>
        public async Task<IActionResult> DeleteDokument(string zakazkaTG, string subor, string skupina)
        {
            short.TryParse(skupina, out short mySkupina);
            if (mySkupina == 0)
                return null;

            Zakazka z1 = await (_context.Zakazkas
                     .Where(z => z.ZakazkaTg == zakazkaTG)
                     .Include(z => z.Dokuments)
                     ).FirstOrDefaultAsync();

            //dokDB moze byt Dokument pre ZakazkaTGdokument, ZakazkaTBdokument, alebo Povinny alebo Priloha;
            Dokument? dokDB = z1.Dokuments.FirstOrDefault(z => z.Skupina == mySkupina);//Dokument, ktory sa ma vymazat
            if (dokDB != null)
            {
                _context.Dokuments.Remove(dokDB);
                int pp = await _context.SaveChangesAsync();
            }
            //NACITANIE vsetkych Dokumentov a DokumentDetail-ov pre vybratu zakazku
            Zakazka zakazkaDB = await _context.Zakazkas
                .Where(m => m.ZakazkaTg == zakazkaTG)
               .Include(z => z.Dokuments)
               .ThenInclude(d => d.DokumentDetails)
               .OrderByDescending(z => z.Vytvorene)
               .FirstOrDefaultAsync();    //NACITANIE vsetkych Dokumentov a DokumentDetail-ov pre vybratu zakazku

            if (zakazkaDB == null)
                return RedirectToAction(nameof(Index));

            if (zakazkaDB.Ukoncena == "A")//test ci sa nevymazal Povinny dokument
            {
                int pocetPovinnych = zakazkaDB?.Dokuments?.Count(d => !string.IsNullOrEmpty(d.NazovDokumentu) && d.Skupina > 2 && d.Skupina < 100) ?? 0;
                if (pocetPovinnych < 5)
                {
                    zakazkaDB.Ukoncena = "N";
                }
            }
            

            //ZakazkaZO je ViewModel pre instanciu  typu Zakazka
            ZakazkaZO zakazkaZO = _transformService.ConvertZakazka_To_ZakazkaZO(ref zakazkaDB);
            return View(nameof(UpdateZakazka), zakazkaZO); //vrati UpdateZakazka.cshtml s viemodelom zakazkaZO
            
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

            if(Global.AppData.AutoOpenFile)
            {
                return File(filecontent, mimeType);
            }
            //

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
        //Vytvorenie novej zakazky, Pre vsetky roly okrem roly READONLY!
        // volanie: <a asp-action="NovaZakazka" style="margin-left:50px;">Nová garančná oprava</a>
        public IActionResult NovaZakazka()  //pouziva Views\ZakazkyJQ\NovaZakazka.cshtml  
        {
            short pocetPriloh = 1;
            ZakazkaZO zakazkaZO = _transformService.VytvorPrazdnuZakazkuZO(pocetPriloh: pocetPriloh);

            return View(zakazkaZO);
        }

        //Volanie z NovaZakazka.cshtml po kliku na button "Ulozit", po zadani parametrov pre novu zakazku;
        //Volanie z NovaZakazka.cshtml po kliku na button "Vymazat subor", po vybere suboru pre novu zakazku;
        //Volanie z NovaZakazka.cshtml po kliku na button "Pridaj prilohu",  pre pridanie prilohy pre novu zakazku;
        //Nahratie suboru z klienta na server a ulozenie do DB
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( ZakazkaZO zakazkaZO)
        {
            var vybrataSkupina = Request.Form["vybrataSkupina"].FirstOrDefault();//vybrata  (skupina > 0) && (skupina < 200) bol klik na button na mazanie suboru

            _ = short.TryParse(vybrataSkupina, out short skupina);
            /*
             * skupina=1 mazanie obrazku pre ZakazkaTGdokument
               skupina=2 mazanie obrazku pre ZakazkaTBdokument
               skupina=20,21,22,23,24  mazanie obrazku pre Povinny dokument
               skupina=100,101,...  mazanie obrazku pre Prilohu

               skupina = 222 pridanie prilohy;
             */
           

            if (skupina == 0)//nebol klik na button na mazanie suboru; zakazkaZO sa ulozi do DB
            { 
                bool mv = ModelState.IsValid;
                if (ModelState.IsValid)//ulozim zakazkuZO do DB
                {
                    int pd = NastavZakazkuZO( ref zakazkaZO );  
                    try
                    {
                        //Z instancie typu ZakazkaZO vytvorime instanciu typu Zakazka a pridame ju do _contextu
                        Zakazka novaZakazkaDB = _transformService.ConvertZakazkaZO_To_NewZakazka(ref zakazkaZO);
                        _context.Add(novaZakazkaDB);
                        int pocetUlozenych = await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                    catch //ak nastane exception pri spracovani, napr. zada sa ZakazkaTG, ktore uz existuje v DB!
                    {
                        //string msg = ex.Message;
                        //chybovy oznam sa zobrazi v hornej casti stranky
                        zakazkaZO.ErrorMessage = $"!! Nastala chyba pri uložení záznamu pre ZakazkaTg: {zakazkaZO.ZakazkaTg} !!";  //ex.Message;
                    }
                }// if (ModelState.IsValid)

                return View("NovaZakazka", zakazkaZO);
            }
            else if ((skupina > 0) && (skupina < 200))
            {
                //nastavi zakazkuZO a vymaze dokument pre skupinu 
                int pd = NastavZakazkuZO(ref zakazkaZO);//nema este nastaveny ani jeden dokument
                if (pd == 0)
                {
                    return View("NovaZakazka", zakazkaZO); ;
                }
                int pd2 = VymazDokument(ref zakazkaZO, skupinaDokumentu: skupina);
                return View("NovaZakazka", zakazkaZO);
            }
            else if(skupina==222) //priznak ze bol klik na button 'Pridaj prilohu'
            {
                int pd = NastavZakazkuZO(ref zakazkaZO);//nastavi zakazku podla prijatych udajov, vrati pocet dokumentov ktore maju nastavenu prilohu;
                //pridanie polozky do Priloh
                 short maxSkupina = zakazkaZO?.Prilohy.Max(d => d.Skupina) ?? 0;//prilohy maju skupinu: 100,101,102,...
                if (maxSkupina > 0)  
                {
                    //napr. max Skupina=104
                    short novaSkupina = (short)(maxSkupina + 1);
                    zakazkaZO.Prilohy.Add(new BaseItem() { NazovDokumentu=$"Priloha{(novaSkupina+1)%100:00}",  Skupina = novaSkupina });
                }
               //niekde nastala chyba: maxSkupina==0;
            }
            return View("NovaZakazka", zakazkaZO);
        }

        /*
         * Pre Vymazanie suboru sa vola DeleteDokument
         */ 

        //UpdateZakazka.cshtml klik na button"Ulozit", po zadani parametrov pre zmenenu zakazku;
        //Volanie z UpdateZakazka.cshtml po kliku na button "Pridaj prilohu",  pre pridanie prilohy pre novu zakazku;
        //Nahratie suboru z klienta na server a ulozenie do DB
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateZakazka(ZakazkaZO zakazkaZO)
        {
            var vybrataSkupina = Request.Form["vybrataSkupina"].FirstOrDefault();//vybrataSkupina >0 bol klik na button na pridanie prilohy
            _ = short.TryParse(vybrataSkupina, out short skupina);


            if (skupina == 0)//nebol klik na button pre pridanie Prilohy
            {
                bool mv = ModelState.IsValid;

                if (ModelState.IsValid)
                {
                    // zakazkaZO.ErrorMessage = $"!! Update funkcionalita este nie je dokoncena pre ZakazkaTg: {zakazkaZO.ZakazkaTg} !!";


                    string zakazkaTg = zakazkaZO.ZakazkaTg;
                    Zakazka povodnaZakazkaDB = await _context.Zakazkas
                    .Where(m => m.ZakazkaTg == zakazkaTg)
                   .Include(z => z.Dokuments)
                   .ThenInclude(d => d.DokumentDetails)
                   .OrderByDescending(z => z.Vytvorene)
                   .FirstOrDefaultAsync();    //NACITANIE vsetkych Dokumentov a DokumentDetail-ov pre vybratu zakazku

                    int pd = NastavZakazkuZO(ref zakazkaZO);
                    /*
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

                    foreach (var pd in zakazkaZO.PovinneDokumenty)
                    {
                        if (pd.DokFormFile == null)//nebpl vybray subor
                            continue;
                        using (var ms = new MemoryStream())
                        {
                            pd.DokFormFile.CopyTo(ms);
                            pd.FileContent = ms.ToArray();
                            pd.NazovSuboru = pd.DokFormFile.FileName;
                        }
                    }

                    foreach (var prd in zakazkaZO.Prilohy)
                    {
                        if (prd.DokFormFile == null)
                            continue;
                        using (var ms = new MemoryStream())
                        {
                            prd.DokFormFile.CopyTo(ms);
                            prd.FileContent = ms.ToArray();
                            prd.NazovSuboru = prd.DokFormFile.FileName;
                        }
                    }
                    */
                    try
                    {
                        _transformService.ConvertZakazkaZO_To_Zakazka(ref zakazkaZO, ref povodnaZakazkaDB);

                        int pocetUlozenych = await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                    catch //ak nastane exception pri spracovani, napr. zada sa ZakazkaTG, ktore uz existuje v DB!
                    {
                        //string msg = ex.Message;
                        //chybovy oznam sa zobrazi v hornej casti stranky
                        zakazkaZO.ErrorMessage = $"!! Nastala chyba pri uložení záznamu pre ZakazkaTg: {zakazkaZO.ZakazkaTg} !!";  //ex.Message;
                    }

                }//if (ModelState.IsValid)
                return View(zakazkaZO);
            }
            
            else if (skupina == 222) //priznak ze bol klik na button 'Pridaj prilohu'
            {
                //pridanie polozky do Priloh
                short maxSkupina = zakazkaZO.Prilohy.Max(d => d.Skupina) ?? 0;
                if (maxSkupina > 0)
                {
                    //napr. max Skupina=104
                    short novaSkupina = (short)(maxSkupina + 1);
                    zakazkaZO.Prilohy.Add(new BaseItem() { NazovDokumentu = $"Priloha{(novaSkupina + 1) % 100:00}", Skupina = novaSkupina });
                }
                //niekde nastala chyba
            }
            return View(zakazkaZO);
        }

        #region ==stary kod ==
        /*
         * //UpdateZakazka.cshtml klik na button"Ulozit", po zadani parametrov pre zmenenu zakazku;
        //Volanie z UpdateZakazka.cshtml po kliku na button "Pridaj prilohu",  pre pridanie prilohy pre novu zakazku;
        //Nahratie suboru z klienta na server a ulozenie do DB
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateZakazka(ZakazkaZO zakazkaZO)
        {
            var vybrataSkupina = Request.Form["vybrataSkupina"].FirstOrDefault();//vybrataSkupina >0 bol klik na button na pridanie prilohy
            _ = short.TryParse(vybrataSkupina, out short skupina);


            if (skupina == 0)//nebol klik na button pre pridanie Prilohy
            {
                bool mv = ModelState.IsValid;

                if (ModelState.IsValid)
                {
                    // zakazkaZO.ErrorMessage = $"!! Update funkcionalita este nie je dokoncena pre ZakazkaTg: {zakazkaZO.ZakazkaTg} !!";


                    string zakazkaTg = zakazkaZO.ZakazkaTg;
                    Zakazka povodnaZakazkaDB = await _context.Zakazkas
                    .Where(m => m.ZakazkaTg == zakazkaTg)
                   .Include(z => z.Dokuments)
                   .ThenInclude(d => d.DokumentDetails)
                   .OrderByDescending(z => z.Vytvorene)
                   .FirstOrDefaultAsync();    //NACITANIE vsetkych Dokumentov a DokumentDetail-ov pre vybratu zakazku

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

                    foreach (var pd in zakazkaZO.PovinneDokumenty)
                    {
                        if (pd.DokFormFile == null)//nebpl vybray subor
                            continue;
                        using (var ms = new MemoryStream())
                        {
                            pd.DokFormFile.CopyTo(ms);
                            pd.FileContent = ms.ToArray();
                            pd.NazovSuboru = pd.DokFormFile.FileName;
                        }
                    }

                    foreach (var prd in zakazkaZO.Prilohy)
                    {
                        if (prd.DokFormFile == null)
                            continue;
                        using (var ms = new MemoryStream())
                        {
                            prd.DokFormFile.CopyTo(ms);
                            prd.FileContent = ms.ToArray();
                            prd.NazovSuboru = prd.DokFormFile.FileName;
                        }
                    }

                    try
                    {
                        _transformService.ConvertZakazkaZO_To_Zakazka(ref zakazkaZO, ref povodnaZakazkaDB);

                        int pocetUlozenych = await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                    catch //ak nastane exception pri spracovani, napr. zada sa ZakazkaTG, ktore uz existuje v DB!
                    {
                        //string msg = ex.Message;
                        //chybovy oznam sa zobrazi v hornej casti stranky
                        zakazkaZO.ErrorMessage = $"!! Nastala chyba pri uložení záznamu pre ZakazkaTg: {zakazkaZO.ZakazkaTg} !!";  //ex.Message;
                    }

                }//if (ModelState.IsValid)
                return View(zakazkaZO);
            }
            else if (skupina == 111) //priznak ze bol klik na button 'Pridaj prilohu'
            {
                //pridanie polozky do Priloh
                short maxSkupina = zakazkaZO.Prilohy.Max(d => d.Skupina) ?? 0;
                if (maxSkupina > 0)
                {
                    //napr. max Skupina=104
                    short novaSkupina = (short)(maxSkupina + 1);
                    zakazkaZO.Prilohy.Add(new BaseItem() { NazovDokumentu = $"Priloha{(novaSkupina + 1) % 100:00}", Skupina = novaSkupina });
                }
                //niekde nastala chyba
            }
            return View(zakazkaZO);
        }
         * */
        #endregion ==stary kod ==

        /// <summary>
        /// Vymaze udaje o dokumente z viemodelu ZakazkaZO podla zadanej hodnoty skupina zo
        /// </summary>
        /// <param name="zakazkaZO"></param>
        /// <param name="skupinaDokumentu"></param>
        /// <returns>Pocet dokumentov, ktore maju nastaveny NazovDokumentu</returns>
        private int VymazDokument( ref ZakazkaZO zakazkaZO, short skupinaDokumentu)
        {
            int pocetDokumentov = 0;
            if (skupinaDokumentu > 0)
            {
                //int pd = NastavZakazkuZO(ref zakazkaZO);//nema este nastaveny ani jeden dokument
                //if( pd == 0)
                //{
                //    return 0;
                //}
                if (skupinaDokumentu == 1)
                {
                    zakazkaZO.ZakazkaTGdokument.FileContent = null;
                    zakazkaZO.ZakazkaTGdokument.NazovSuboru = null;
                }
                else if (skupinaDokumentu == 2)
                {
                    zakazkaZO.ZakazkaTBdokument.FileContent = null;
                    zakazkaZO.ZakazkaTBdokument.NazovSuboru = null;
                }
                else if (skupinaDokumentu >= 20 && skupinaDokumentu < 100)//Povinne dokumenty
                {
                    int pp = zakazkaZO.PovinneDokumenty.Count;  //20,21,22,23,24
                    int index = skupinaDokumentu - 20;
                    if(index<pp)
                    {
                        zakazkaZO.PovinneDokumenty[index].NazovSuboru = null;
                        zakazkaZO.PovinneDokumenty[index].FileContent = null;
                    }
                }
                else if (skupinaDokumentu >= 100)//Prilohy
                {
                    int pp = zakazkaZO.Prilohy.Count;  //100,101,102,103,104,....
                    int index = skupinaDokumentu - 100;
                    if (index < pp)
                    {
                        zakazkaZO.Prilohy[index].NazovSuboru = null;
                        zakazkaZO.Prilohy[index].FileContent = null;
                    }
                }

                pocetDokumentov = (zakazkaZO?.PovinneDokumenty?.Count(p => string.IsNullOrEmpty(p.NazovDokumentu)) ?? 0) + (zakazkaZO?.Prilohy?.Count(p => string.IsNullOrEmpty(p.NazovDokumentu)) ?? 0);
                if (!string.IsNullOrEmpty(zakazkaZO.ZakazkaTGdokument.NazovSuboru))
                {
                    pocetDokumentov++;
                }
                if (!string.IsNullOrEmpty(zakazkaZO.ZakazkaTBdokument.NazovSuboru))
                {
                    pocetDokumentov++;
                }
            }

            return pocetDokumentov;
        }

        /// <summary>
        /// Nastavi NazovDokumentu a FileContent pre dokument, ktory ma nastaveny subor;
        /// </summary>
        /// <param name="zakazkaZO"></param>
        /// <returns>Pocet dokumentov, ktore maju nastaveny NazovDokumentu</returns>
        private int NastavZakazkuZO( ref ZakazkaZO zakazkaZO)
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

            //zakazkaZO moze mat nastavene subory pre povinne dokumenty
            foreach (BaseItem d in zakazkaZO.PovinneDokumenty)//nastavenie povinnych
            {
                if (d.DokFormFile == null)
                    continue;
                using (var ms = new MemoryStream())
                {
                    d.DokFormFile.CopyTo(ms);
                    d.FileContent = ms.ToArray();
                    d.NazovSuboru = d.DokFormFile.FileName;
                }
            }
            //zakazkaZO moze mat nastavene subory pre prilohy
            foreach (BaseItem d in zakazkaZO.Prilohy)
            {
                if (d.DokFormFile == null)
                    continue;
                using (var ms = new MemoryStream())
                {
                    d.DokFormFile.CopyTo(ms);
                    d.FileContent = ms.ToArray();
                    d.NazovSuboru = d.DokFormFile.FileName;
                }
            }
            int pocetDokumentov = (zakazkaZO?.PovinneDokumenty?.Count(p => string.IsNullOrEmpty(p.NazovDokumentu))  ?? 0) + ( zakazkaZO?.Prilohy?.Count(p =>string.IsNullOrEmpty(p.NazovDokumentu)) ?? 0) ;
            if( !string.IsNullOrEmpty(zakazkaZO.ZakazkaTGdokument.NazovSuboru))
            {
                pocetDokumentov++;
            }
            if (!string.IsNullOrEmpty(zakazkaZO.ZakazkaTBdokument.NazovSuboru))
            {
                pocetDokumentov++;
            }
            return pocetDokumentov;
        }
    }
}
