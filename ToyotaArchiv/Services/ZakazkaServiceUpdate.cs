using PA.TOYOTA.DB;
using ToyotaArchiv.Domain;
using ToyotaArchiv.Infrastructure;

namespace ToyotaArchiv.Services
{
    //MH: april 2022
    //pozri aj ZakazkaService.cs
    internal partial class ZakazkaServiceWeb : IZakazkaTransformService
    {

        /*UPDATE instancie typu Zakazka
        * Postup pru UPDATE
        * Z  DB bola nacitana instancia typu Zakazka;
        * Z nej som vytvoril instanciu typu ZakazkaZO (je to viewModel pre zobrazenie vo View )
        * Instancia typu ZakazkaZO je zobrazena vo view a uzivatel mohol zmenit udaje...a stlacil "Ulozit zmeny".
        * Teraz musim preniest zmeny z upravenej instancie typu ZakazkaZO (view modelu) do povodnej instancie typu Zakazka a volat _context.SaveChanges();
        * 
        * 
        * Button "Clear" vymaze dokument pre danu skupinu a refresne stranku
        * Ak ZakazkaZO dokument.NazovSuboru je zadany a DoFormFile = null, znamena ze sa subor NEZMENIL!!!
        * Ak ZakazkaZO dokument.NazovSuboru nie je zadany DoFormFile != null, bol pridany obrazok !!!
        * 
        * 
        * Pri editovani zakazkaZO sa mohli zmenit obsahy  dokumentov,
        * mohol pribudnut dokument (pridal sa subor) do zakazkaZO, mohol sa odobrat dokument (vymazal sa subor), mohla sa zmeit poznamka;
        */

        /// <summary>
        /// Prenesie udaje z ViewModelu do entity typu Zakazka;
        /// </summary>
        /// <param name="zakazkaZO">View model</param>
        /// <param name="zakazkaDB">Entita typu Zakazka, na ktorej sa robi update</param>
        /// <exception cref="ArgumentNullException"></exception>
        public void ConvertZakazkaZO_To_Zakazka(ref ZakazkaZO zakazkaZO, ref Zakazka zakazkaDB)  //ked sa robi UPDATE zaznamu z DB!!!!
        {
            if (zakazkaZO == null)
            {
                throw new ArgumentNullException($"Chyba-ConvertZakazkaZO_To_Zakazka: Neplatný údaj pre vstupnú zákazku!");
            }
            if (zakazkaDB == null)
            {
                throw new ArgumentNullException($"Chyba-ConvertZakazkaZO_To_Zakazka: Neplatný údaj pre výstupnú zákazku!");
            }

            //zakazkaDB.ZakazkaTg = myZakZO.ZakazkaTg;   //ZakazkaTg sa nemeni!!!
            zakazkaDB.ZakazkaTb = zakazkaZO?.ZakazkaTb?.Trim();

            zakazkaDB.Poznamka = zakazkaZO?.Poznamka?.Trim();
            zakazkaDB.Ukoncena = zakazkaZO?.Ukoncena;
            zakazkaDB.Vin = zakazkaZO?.Vin?.Trim();
            zakazkaDB.Cws = zakazkaZO?.Cws?.Trim();
            zakazkaDB.CisloProtokolu = zakazkaZO?.CisloProtokolu?.Trim();
            zakazkaDB.Spz = zakazkaZO?.SPZ?.Trim();
            zakazkaDB.Vlastnik = zakazkaZO?.Vlastnik?.Trim();

            #region == Dokumenty  pre Skupina=1 a Skupina=2 ===

            Dokument? zakazkaTGdokumentDB = zakazkaDB.Dokuments.FirstOrDefault(d => d.Skupina == SkupinaZakazkaTGDokument);
            if (zakazkaTGdokumentDB != null)//zakazkaDB mala uz vytvoreny zakazkaTGdokument, 
            {
                zakazkaTGdokumentDB.Poznamka = zakazkaZO?.ZakazkaTGdokument?.Poznamka?.Trim();
                if (zakazkaZO?.ZakazkaTGdokument?.DokFormFile != null)  //nastala zmena obrazku
                {
                    using (var ms = new MemoryStream())
                    {
                        zakazkaTGdokumentDB.NazovSuboru = zakazkaZO?.ZakazkaTGdokument.NazovSuboru?.Trim();  //ZakazkaTGdokument.DokFormFile.FileName;
                        zakazkaZO?.ZakazkaTGdokument.DokFormFile.CopyTo(ms);

                        DokumentDetail? detailDB = zakazkaTGdokumentDB.DokumentDetails.FirstOrDefault(d => d.Skupina == SkupinaZakazkaTGDokument);
                        if (detailDB != null && detailDB.DokumentContent != null)
                        {
                            detailDB.DokumentContent = ms.ToArray();
                        }
                    }
                }
            }
            else//zakazkaDB ak bola uz ulozena nemusi mat este vytvoreny ZakazkaTGdokument!!  Dokument pre Skupina=1
            {
                //pre ZakazkaTGdokument sa nenastavuje poznamka
                if (zakazkaZO?.ZakazkaTGdokument?.DokFormFile != null) //pre zakazkaZO bol zadany obrazok pre ZakazkaTGdokument;
                {
                    Dokument dokumentDB = new Dokument();
                    dokumentDB.ZakazkaTg = zakazkaZO.ZakazkaTg;
                    dokumentDB.NazovDokumentu = zakazkaZO?.ZakazkaTGdokument?.NazovDokumentu;
                    dokumentDB.NazovSuboru = zakazkaZO?.ZakazkaTGdokument.NazovSuboru;
                    dokumentDB.Skupina = SkupinaZakazkaTGDokument;

                    DokumentDetail detailDB = new DokumentDetail(); //pre dokument1 vytvorime DokumentDetail
                    detailDB.Skupina = dokumentDB?.Skupina;

                    using (var ms = new MemoryStream())
                    {
                        zakazkaZO?.ZakazkaTGdokument.DokFormFile.CopyTo(ms);
                        detailDB.DokumentContent = ms.ToArray();
                    }
                    if (dokumentDB != null)
                    {
                        dokumentDB?.DokumentDetails.Add(detailDB);
                        zakazkaDB.Dokuments.Add(dokumentDB);
                    }
                }
            }

            Dokument? zakazkaTBdokumentDB = zakazkaDB.Dokuments.FirstOrDefault(d => d.Skupina == SkupinaZakazkaTBDokument);
            if (zakazkaTBdokumentDB != null)//zakazkaDB mala uz zakazkaTBdokument, musi mat DokumentDetail
            {
                //pre ZakazkaTBdokument sa nenastavuje poznamka
                //zakazkaTBdokumentDB.Poznamka = zakazkaZO?.ZakazkaTBdokument?.Poznamka?.Trim();

                if (zakazkaZO?.ZakazkaTBdokument?.DokFormFile != null)  //nastala zmena obrazku
                {
                    using (var ms = new MemoryStream())
                    {
                        zakazkaTBdokumentDB.NazovSuboru = zakazkaZO.ZakazkaTBdokument.NazovSuboru;
                        zakazkaZO.ZakazkaTBdokument.DokFormFile.CopyTo(ms);

                        DokumentDetail? detailDB = zakazkaTBdokumentDB.DokumentDetails.FirstOrDefault(d => d.Skupina == SkupinaZakazkaTBDokument);
                        if (detailDB != null && detailDB.DokumentContent != null)
                        {
                            detailDB.DokumentContent = ms.ToArray();//nastala zmena dokumentu
                        }
                    }
                }
            }
            else//zakazkaDB este nemala vytvoreny ZakazkaTBdokumentDB pre Skupina=2
            {
                if (zakazkaZO?.ZakazkaTBdokument?.DokFormFile != null) //pre myZakZO bol zadany obrazok pre ZakazkaTBdokument;
                {
                    Dokument dokumentDB = new Dokument();
                    dokumentDB.ZakazkaTg = zakazkaZO.ZakazkaTg;
                    dokumentDB.NazovDokumentu = zakazkaZO.ZakazkaTBdokument.NazovDokumentu;
                    dokumentDB.NazovSuboru = zakazkaZO.ZakazkaTBdokument.NazovSuboru;
                    dokumentDB.Skupina = SkupinaZakazkaTBDokument;

                    DokumentDetail detailDB = new DokumentDetail(); //pre dokument1 vytvorime DokumentDetail
                    detailDB.Skupina = dokumentDB?.Skupina;

                    using (var ms = new MemoryStream())
                    {
                        zakazkaZO.ZakazkaTBdokument.DokFormFile.CopyTo(ms);
                        detailDB.DokumentContent = ms.ToArray();
                    }
                    dokumentDB?.DokumentDetails.Add(detailDB);
                    zakazkaDB.Dokuments.Add(dokumentDB);
                }
            }

            #endregion == Dokumenty  pre Skupina=1 a Skupina=2 ===

            #region == Povinne dokumenty ==

            List<BaseItem>? povinneDokumentyZO = null;
            List<Dokument>? povinneDokumentyDB = null;

            bool existujuPovinneDokumentyZO = zakazkaZO?.PovinneDokumenty?.Any(d => !string.IsNullOrEmpty(d.NazovSuboru)) ?? false;
            if (existujuPovinneDokumentyZO)
                povinneDokumentyZO = zakazkaZO?.PovinneDokumenty?.Where(d => !string.IsNullOrEmpty(d.NazovSuboru)).ToList();

            bool existujuPovinneDokumentyDB = zakazkaDB.Dokuments.Any(d => d.Skupina >= SkupinaPrvehoPovinnehoDokumentu && d.Skupina < SkupinaPrvejPrilohy);

            if (existujuPovinneDokumentyDB)
                povinneDokumentyDB = zakazkaDB.Dokuments.Where(d => d.Skupina >= SkupinaPrvehoPovinnehoDokumentu && d.Skupina < SkupinaPrvejPrilohy).ToList();

            if (!existujuPovinneDokumentyDB && existujuPovinneDokumentyZO)//zakazkaDB este nemala vytvorene Povinne dokumenty
            {
                //vytvorim nove dokumenty z povinneDokumentyZO a vlozim ich do  zakazkaDB.Dokuments.
                if (povinneDokumentyZO != null)
                {
                    foreach (BaseItem pdZO in povinneDokumentyZO)
                    {
                        Dokument dokumentDB = new Dokument();
                        dokumentDB.ZakazkaTg = zakazkaZO.ZakazkaTg.Trim();
                        dokumentDB.NazovDokumentu = pdZO.NazovDokumentu;
                        dokumentDB.NazovSuboru = pdZO.NazovSuboru;
                        dokumentDB.Skupina = pdZO.Skupina;
                        dokumentDB.Poznamka = pdZO?.Poznamka?.Trim();

                        DokumentDetail detailDB = new DokumentDetail();
                        detailDB.Skupina = pdZO.Skupina;
                        detailDB.DokumentContent = new byte[pdZO.FileContent.Length];
                        pdZO.FileContent.CopyTo(detailDB.DokumentContent, 0);

                        dokumentDB.DokumentDetails.Add(detailDB);
                        zakazkaDB.Dokuments.Add(dokumentDB);
                    }
                }
            }
            else if (existujuPovinneDokumentyDB && existujuPovinneDokumentyZO) //zakazkaDB uz ma nejake Povinne dokumenty; zakazkaZO ma vzdy 5 Povinnych Dokumentov, ale nemusia mat zadany NazovSuboru
            {
                //zoznam skupin pre povinne dokumenty z DB
                var zoznamSkupinDB = zakazkaDB.Dokuments.Where(d => d.Skupina >= 20 && d.Skupina < 100).Select(d => d.Skupina).ToList();//Zoznam skupin pre PovinneDokumenty z DB
                var zoznamSkupinZO = povinneDokumentyZO?.Select(d => d.Skupina).ToList(); //Zoznam skupin z povinneDokumentyZO; povinneDokumentyZO su zaznamy so zadanym suborom

                bool skupinaZOexistujeInZSDB = false;
                if (zoznamSkupinZO != null)
                {
                    foreach (short? skZO in zoznamSkupinZO)
                    {
                        skupinaZOexistujeInZSDB = zoznamSkupinDB.Any(s => s == skZO);
                        if (!skupinaZOexistujeInZSDB) //vytvorim novy dokument z povinneDokumentyZO a pridam ho do  zakazkaDB.Dokuments.
                        {
                            BaseItem? pdZO = povinneDokumentyZO?.FirstOrDefault(d => d.Skupina == skZO);
                            if (pdZO != null) //z pdZO vytvorim novy dokumentDB
                            {

                                Dokument dokumentDB = new Dokument();
                                dokumentDB.ZakazkaTg = zakazkaZO.ZakazkaTg.Trim();
                                dokumentDB.NazovDokumentu = pdZO.NazovDokumentu;
                                dokumentDB.NazovSuboru = pdZO.NazovSuboru;
                                dokumentDB.Skupina = pdZO.Skupina;
                                dokumentDB.Poznamka = pdZO?.Poznamka?.Trim();

                                DokumentDetail detailDB = new DokumentDetail();
                                detailDB.Skupina = pdZO?.Skupina;

                                if (pdZO?.FileContent != null)
                                {
                                    detailDB.DokumentContent = new byte[pdZO.FileContent.Length];
                                    pdZO.FileContent.CopyTo(detailDB.DokumentContent, 0);
                                }

                                dokumentDB.DokumentDetails.Add(detailDB);
                                zakazkaDB.Dokuments.Add(dokumentDB);
                            }
                        }
                        else  //urobim update existujuceho zaznamu dokumentDB
                        {
                            Dokument? dokumentDB = zakazkaDB.Dokuments.FirstOrDefault(d => d.Skupina == skZO);
                            BaseItem? pdZO = povinneDokumentyZO?.FirstOrDefault(d => d.Skupina == skZO);
                            if (dokumentDB != null && pdZO != null) //robi sa update-zmena povinneho dokumentu
                            {
                                //dokumentDB.NazovDokumentu sa nemeni
                                //dokumentDB.Skupina sa nemeni
                                dokumentDB.Poznamka = pdZO?.Poznamka?.Trim();

                                if (pdZO?.DokFormFile != null) //zmenil sa obrazok
                                {
                                    dokumentDB.NazovSuboru = pdZO?.NazovSuboru;
                                    DokumentDetail? detailDB = dokumentDB.DokumentDetails.FirstOrDefault(d => d.Skupina == skZO);
                                    if (detailDB != null)
                                    {
                                        if (pdZO?.FileContent != null)
                                        {
                                            detailDB.DokumentContent = new byte[pdZO.FileContent.Length];
                                            pdZO.FileContent.CopyTo(detailDB.DokumentContent, 0);
                                        }
                                    }
                                }
                            }

                        }
                    }
                }

            }//else if (existujuPovinneDokumentyDB && existujuPovinneDokumentyZO)

            else if (existujuPovinneDokumentyDB && !existujuPovinneDokumentyZO) //zakazkaDB uz ma nejake Povinne dokumenty; zakazkaZO nema zadane nazvy suborov pre povinne dokumenty, dokumenty boli vymazane
            {
                ;
            }
           
            
            //nastavenie priznaku: Zakazka Ukoncena
            existujuPovinneDokumentyDB = zakazkaDB.Dokuments.Any(d => d.Skupina >= SkupinaPrvehoPovinnehoDokumentu && d.Skupina < SkupinaPrvejPrilohy);

            if (existujuPovinneDokumentyDB)
            {
                int pd = zakazkaDB?.Dokuments?.Count(d => d.Skupina >= SkupinaPrvehoPovinnehoDokumentu && d.Skupina < SkupinaPrvejPrilohy) ?? 0;
                if (zakazkaDB != null)
                {
                    if (pd == PocetPovinnych)
                        zakazkaDB.Ukoncena = "A";
                    else
                        zakazkaDB.Ukoncena = "N";
                }
            }

            #endregion == Povinne dokumenty ==

            #region == Prilohy ==

            List<BaseItem>? prilohyZO = null;
            List<Dokument>? prilohyDB = null;

            bool existujuPrilohyZO = zakazkaZO?.Prilohy?.Any(d => !string.IsNullOrEmpty(d.NazovSuboru)) ?? false;

            if (existujuPrilohyZO) //ak existuje dokument pre prilohu, je nastavene pole pre obrazok
                prilohyZO = zakazkaZO?.Prilohy?.Where(d => !string.IsNullOrEmpty(d.NazovSuboru)).ToList();

            bool existujuPrilohyDB = zakazkaDB?.Dokuments.Any(d => d.Skupina >= SkupinaPrvejPrilohy) ?? false;

            if (existujuPrilohyDB)
                prilohyDB = zakazkaDB.Dokuments.Where(d => d.Skupina >= SkupinaPrvejPrilohy).ToList();

            if (!existujuPrilohyDB && existujuPrilohyZO)//zakazkaDB este nemala vytvorene Prilohy
            {
                //vytvorim nove dokumenty pre zakazkaDB a pridam ich do zakazkaDB.Dokuments.
                if (prilohyZO != null)
                {
                    foreach (BaseItem dokumentZO in prilohyZO)
                    {
                        Dokument dokumentDB = new Dokument();
                        dokumentDB.ZakazkaTg = zakazkaZO.ZakazkaTg.Trim();
                        dokumentDB.NazovDokumentu = dokumentZO.NazovDokumentu;
                        dokumentDB.NazovSuboru = dokumentZO.NazovSuboru;
                        dokumentDB.Skupina = dokumentZO.Skupina;
                        dokumentDB.Poznamka = dokumentZO?.Poznamka?.Trim();

                        DokumentDetail dokDetailDB = new DokumentDetail();
                        dokDetailDB.Skupina = dokumentZO.Skupina;
                        if (dokumentZO.FileContent != null)
                        {
                            dokDetailDB.DokumentContent = new byte[dokumentZO.FileContent.Length];
                            dokumentZO.FileContent.CopyTo(dokDetailDB.DokumentContent, 0);

                            dokumentDB.DokumentDetails.Add(dokDetailDB);
                            zakazkaDB?.Dokuments.Add(dokumentDB);
                        }
                    }
                }
            }
            else if (existujuPrilohyDB && existujuPrilohyZO)//zakazkaDB uz ma nejake Prilohy a zakazkaZO ma nejake prilohy;
            {
                //Co ak zakazkaZO ma viac priloh ako zakazkaDB, pridam ich do zakazkaDB.Dokuments!!

                //zoznam skupin pre povinne dokumenty z DB
                List<short? >? zoznamSkupinDB = zakazkaDB?.Dokuments?.Where(d => d.Skupina >= 100).Select(d => d.Skupina).ToList();//Zoznam skupin pre Prilohy z DB
                List<short?>? zoznamSkupinZO = prilohyZO?.Select(d => d.Skupina).ToList(); //Zoznam skupin z Prilohy; prilohyZO su zaznamy so zadanym NazovSuboru
               
                bool skupinaZOexistujeInZSDB = false;
                if (zoznamSkupinZO != null)
                {
                    foreach (short? skZO in zoznamSkupinZO)
                    {
                        skupinaZOexistujeInZSDB = zoznamSkupinDB?.Any(s => s == skZO) ?? false;
                        if (!skupinaZOexistujeInZSDB) //vytvorim novy dokument z povinneDokumentyZO a pridam ho do  zakazkaDB.Dokuments.
                        {
                            BaseItem? pdZO = prilohyZO?.FirstOrDefault(d => d.Skupina == skZO);
                            if (pdZO != null) //z pdZO vytvorim novy dokumentDB
                            {

                                Dokument dokumentDB = new Dokument();
                                dokumentDB.ZakazkaTg = zakazkaZO.ZakazkaTg.Trim();
                                dokumentDB.NazovDokumentu = pdZO.NazovDokumentu;
                                dokumentDB.NazovSuboru = pdZO.NazovSuboru;
                                dokumentDB.Skupina = pdZO.Skupina;
                                dokumentDB.Poznamka = pdZO?.Poznamka?.Trim();

                                DokumentDetail detailDB = new DokumentDetail();
                                detailDB.Skupina = pdZO?.Skupina;

                                detailDB.DokumentContent = new byte[pdZO.FileContent.Length];
                                pdZO.FileContent.CopyTo(detailDB.DokumentContent, 0);

                                dokumentDB.DokumentDetails.Add(detailDB);
                                zakazkaDB.Dokuments.Add(dokumentDB);
                            }
                        }
                        else  //urobim update existujuceho zaznamu dokumentDB
                        {
                            Dokument? dokumentDB = zakazkaDB?.Dokuments.FirstOrDefault(d => d.Skupina == skZO);
                            BaseItem? pdZO = prilohyZO?.FirstOrDefault(d => d.Skupina == skZO);
                            if (dokumentDB != null && pdZO != null) //robi sa update-zmena povinneho dokumentu
                            {
                                //dokumentDB.NazovDokumentu sa nemeni
                                //dokumentDB.Skupina sa nemeni

                                if ((dokumentDB.Poznamka != null) && (pdZO.Poznamka != null))
                                {
                                    if (dokumentDB.Poznamka.Trim() != pdZO.Poznamka.Trim())
                                        dokumentDB.Poznamka = pdZO?.Poznamka?.Trim();
                                }
                                else
                                {
                                    dokumentDB.Poznamka = pdZO?.Poznamka?.Trim();
                                }
                                if (pdZO?.DokFormFile != null) //zmenil sa obrazok
                                {
                                    dokumentDB.NazovSuboru = pdZO?.NazovSuboru;
                                    DokumentDetail? detailDB = dokumentDB.DokumentDetails.FirstOrDefault(d => d.Skupina == skZO);
                                    if (detailDB != null)
                                    {
                                        detailDB.DokumentContent = new byte[pdZO.FileContent.Length];
                                        pdZO.FileContent.CopyTo(detailDB.DokumentContent, 0);
                                    }
                                }
                            }

                        }
                    }
                }
               
            }//else if (existujuPrilohyDB && existujuPrilohyZO)

            #endregion == Prilohy ==

        }//ConvertZakazkaZO_To_Zakazka(ZakazkaZO myZakZO, ref Zakazka zakazkaDB)

    }//class
}
