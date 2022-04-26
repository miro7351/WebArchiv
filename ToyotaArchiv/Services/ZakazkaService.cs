using ToyotaArchiv.Domain;
using PA.TOYOTA.DB;
using ToyotaArchiv.Infrastructure;

namespace ToyotaArchiv.Services
{
    //MH: 12.04.2022
    internal class ZakazkaServiceWeb : IZakazkaTransformService
    {
        public ZakazkaServiceWeb()
        {

        }

        /* Nazvy pre Povinne dokumenty
         Name = "Pred kalkulacia TG"
         Name = "Rekapitulacia TG", 
         Name = "Garancna TG", 
         Name = "Registracia TG",
         Name = "Suhrna faktura"

        */

        static List<string> NazvyPovinnychDokumentov = new List<string>() { "Pred kalkulacia TG", "Rekapitulacia TG", "Garancna TG", "Registracia TG", "Suhrna faktura" };

        static short SkupinaZakazkaTGDokument => 1;
        static short SkupinaZakazkaTBDokument => 2;
        static short SkupinaPrvehoPovinnehoDokumentu => 20;
        static short SkupinaPrvejPrilohy => 100;
        static short PocetPovinnych => 5;
        static short PocetPriloh => 5;

        /// <summary>
        /// Typ instancie Zakazka (z databazy) skonveruje do vytvorenej instancie typu ZakazkaZO (pre zobrazenie pre uzivatela);
        /// Ak sa udaje nacitane z databazy maju len zobrazit uzivatelovi;
        /// </summary>
        /// <param name="zakazka">udaj nacitany z databazy</param>
        /// <returns>ZakazkaZO typ pre editaciu alebo zobrazenie</returns>
        public ZakazkaZO ConvertZakazka_To_ZakazkaZO(ref Zakazka zakazka)
        {
            if( zakazka == null)
            {
                throw new ArgumentNullException($"ConvertZakazka_To_ZakazkaZO - Chyba: zle zadaný vstupný parameter!");
            }

            ZakazkaZO zakazkaZO = new ZakazkaZO();
            //zakazkaZO.ZakazkaTGdokument neobsahuje obrazok, tj. ZakazkaTGdokument.FileContent=null
            //obrazok sa na poziadanie vyberie z DB!!!

            zakazkaZO.ZakazkaTg = zakazka.ZakazkaTg.Trim();
            zakazkaZO.ZakazkaTb = zakazka.ZakazkaTb?.Trim();
            zakazkaZO.Poznamka = zakazka?.Poznamka?.Trim();
            zakazkaZO.Ukoncena = zakazka?.Ukoncena;
            zakazkaZO.Vin = zakazka?.Vin?.Trim();
            zakazkaZO.Cws = zakazka?.Cws?.Trim();
            zakazkaZO.CisloProtokolu = zakazka?.CisloProtokolu?.Trim();
            zakazkaZO.Vytvoril = zakazka?.Vytvoril?.Trim();
            zakazkaZO.Vytvorene = zakazka?.Vytvorene;    //DateTime
            zakazkaZO.Zmenil = zakazka?.Zmenil?.Trim(); 
            zakazkaZO.Zmenene = zakazka?.Zmenene;    //DateTime

            //var d1 = zakazka.Dokuments.Where(d => d.Skupina == 1).FirstOrDefault();
            //var detaily = d1.DokumentDetails.FirstOrDefault(d => d.Skupina == 1);
            int pd = zakazka?.Dokuments?.Count ?? 0;
            bool existujuDokumenty = zakazka?.Dokuments.Any(d => !string.IsNullOrEmpty(d.NazovDokumentu)) ?? false;

            if (existujuDokumenty) //existuju zaznamy pre dokumenty
            {
                //Test ci existuje dokument pre zakazkaZO.ZakazkaTGdokument
                if (zakazka.Dokuments.Any(d => d.Skupina == SkupinaZakazkaTGDokument))//vytvorenie zakazkaZO.ZakazkaTGdokument
                {
                    Dokument? dok = zakazka.Dokuments.FirstOrDefault(d => d.Skupina == SkupinaZakazkaTGDokument);
                    DokumentDetail? detail = dok?.DokumentDetails.FirstOrDefault(d => d.Skupina == SkupinaZakazkaTGDokument);

                    zakazkaZO.ZakazkaTGdokument.NazovDokumentu = dok?.NazovDokumentu?.Trim();
                    zakazkaZO.ZakazkaTGdokument.NazovSuboru    = dok?.NazovSuboru?.Trim();

                    zakazkaZO.ZakazkaTGdokument.FileContent = null;

                    zakazkaZO.ZakazkaTGdokument.Skupina   = dok?.Skupina;//short
                    zakazkaZO.ZakazkaTGdokument.Poznamka  = dok?.Poznamka?.Trim();
                }

                //Test ci existuje dokument pre zakazkaZO.ZakazkaTGdokument
                if (zakazka.Dokuments.Any(d => d.Skupina == SkupinaZakazkaTBDokument))//vytvorenie zakazkaZO.ZakazkaTBdokument
                {
                    Dokument? dok = zakazka.Dokuments.FirstOrDefault(d => d.Skupina == SkupinaZakazkaTBDokument);
                   // DokumentDetail detail = dok?.DokumentDetails.FirstOrDefault(d => d.Skupina == SkupinaZakazkaTBDokument);

                    zakazkaZO.ZakazkaTBdokument.NazovDokumentu = dok?.NazovDokumentu?.Trim();
                    zakazkaZO.ZakazkaTBdokument.NazovSuboru = dok?.NazovSuboru?.Trim();

                    zakazkaZO.ZakazkaTGdokument.FileContent = null;
                   
                    zakazkaZO.ZakazkaTBdokument.Skupina = dok?.Skupina;//short
                    zakazkaZO.ZakazkaTBdokument.Poznamka = dok?.Poznamka?.Trim();
                }

                #region ==Povinne dokumenty ==

                if (zakazka.Dokuments.Any(d => d.Skupina >= SkupinaPrvehoPovinnehoDokumentu && d.Skupina < SkupinaPrvejPrilohy))
                {
                    //pocet zaznamov pre povinne dokumety
                    int pz = zakazka.Dokuments.Count(d => d.Skupina >= SkupinaPrvehoPovinnehoDokumentu && d.Skupina < SkupinaPrvejPrilohy);

                    if (pz > 0)  //existuje nejaky povinny dokument
                    {
                        for (short i = 0; i < PocetPovinnych; i++)
                        {
                            Dokument? dok = zakazka.Dokuments.FirstOrDefault(d => d.Skupina == (SkupinaPrvehoPovinnehoDokumentu + i));
                            BaseItem bi = new BaseItem();
                            if (dok != null) //existuje povinny dokument, kde Skupina= (SkupinaPrvehoPovinnehoDokumentu+i)
                            {
                                bi.Skupina = dok.Skupina; //short
                                bi.NazovDokumentu = dok.NazovDokumentu?.Trim();  //
                                bi.Poznamka = dok.Poznamka?.Trim();
                                bi.NazovSuboru = dok.NazovSuboru?.Trim();
                            }
                            else //neexistuje povinny dokument, vytvorim prazdny dokument a nastavim  len jeho property NazovDokumentu a Skupina
                            {
                                bi.Skupina = (short)(SkupinaPrvehoPovinnehoDokumentu + i);//vytvorim jeho skupinu

                                //Ak povinnych dokumentov v DB bolo menej ako NazvyPovinnychDokumentov.Count, potom vytvorim prazdne povinne dokumenty
                                if (i < NazvyPovinnychDokumentov.Count)
                                {
                                    bi.NazovDokumentu = NazvyPovinnychDokumentov[i];
                                }
                                else
                                {
                                    //vytvorim Nazov dokumentu
                                    bi.NazovDokumentu = $"Povinny dok{SkupinaPrvehoPovinnehoDokumentu + i}";
                                }
                            }
                            zakazkaZO.PovinneDokumenty?.Add(bi);//pridam dokument do kolekcie
                        }//for
                    }//if (pz > 0)
                }
                /*
                else  //neexistuju este povinne dokumenty, vytvorim prazdne povinne dokumenty
                {
                    for (short i = 0; i < PocetPovinnych; i++)
                    {
                        BaseItem bi = new BaseItem();
                        bi.Skupina = (short)(SkupinaPrvehoPovinnehoDokumentu + i);//zapisem jeho skupinu

                        if (i < (short)(NazvyPovinnychDokumentov.Count)  ) //zapisem jeho nazov
                        {
                            bi.NazovDokumentu = NazvyPovinnychDokumentov[i];
                        }
                        else
                        {
                            //vytvorim Nazov dokumentu
                            bi.NazovDokumentu = $"Povinny dok{SkupinaPrvehoPovinnehoDokumentu + i}";
                        }
                        zakazkaZO.PovinneDokumenty?.Add(bi);//pridam dokument do kolekcie
                    }
                }
                */
                #endregion ==Povinne dokumenty ==

                #region == Prilohy ==
                if (zakazka.Dokuments.Any(d => d.Skupina >= SkupinaPrvejPrilohy))//Prilohy
                {
                    //pocet zaznamov pre prilohy
                    int pz = zakazka.Dokuments.Count(d => d.Skupina >= SkupinaPrvejPrilohy);

                    if (pz > 0)  //existuju prilohy
                    {
                        var dokumentyPrilohy = zakazka.Dokuments.Where(d => d.Skupina >= SkupinaPrvejPrilohy).ToList();
                        foreach (var dok1 in dokumentyPrilohy)
                        {
                            BaseItem bi = new BaseItem();
                            bi.Skupina = dok1.Skupina;
                            bi.NazovDokumentu = dok1.NazovDokumentu?.Trim();  //
                            
                            bi.Poznamka = dok1.Poznamka?.Trim();
                            bi.NazovSuboru = dok1.NazovSuboru?.Trim();
                            zakazkaZO.Prilohy?.Add(bi);
                        }//for

                        //doplnenie do poctu 10 priloh, nastavim Skupinu a NazovDokumentu PrilohaXX
                        if (PocetPriloh > pz)
                        {
                            for (short i = 0; i < PocetPriloh - pz; i++)
                            {
                                BaseItem bi = new BaseItem();
                                bi.Skupina = (short)(SkupinaPrvejPrilohy + pz + i);
                                bi.NazovDokumentu = $"Priloha{(SkupinaPrvejPrilohy + pz + i) % SkupinaPrvejPrilohy:00}";
                                zakazkaZO.Prilohy?.Add(bi);
                            }
                        }
                    }
                }
                /*
                else //neexistuju prilohy, vytvorim PocetPriloh prazdnych priloh
                {
                    for (short i = 0; i < PocetPriloh; i++)
                    {
                        BaseItem bi = new BaseItem();
                        bi.Skupina = (short)(SkupinaPrvejPrilohy + i);
                        bi.NazovDokumentu = $"Priloha{((SkupinaPrvejPrilohy + i) % SkupinaPrvejPrilohy) + 1:00}"; //Priloha01, Priloha02,...
                        zakazkaZO.Prilohy?.Add(bi);
                    }
                }
                */
                #endregion == Prilohy ==
            }//if( zakazka.Dokuments.Any(d => !string.IsNullOrEmpty(d.NazovDokumentu)) ) //existuju zaznamy pre dokumety
            else
            {  //neexistuju dokumenty pre zakazku

                for (short i = 0; i < PocetPovinnych; i++)//vytvorim Povinne prazdne dokumenty
                {
                    BaseItem bi = new BaseItem();
                    bi.Skupina = (short)(SkupinaPrvehoPovinnehoDokumentu + i);//zapisem jeho skupinu

                    if (i < (short)(NazvyPovinnychDokumentov.Count)) //zapisem jeho nazov
                    {
                        bi.NazovDokumentu = NazvyPovinnychDokumentov[i];
                    }
                    else
                    {
                        //vytvorim Nazov dokumentu
                        bi.NazovDokumentu = $"Povinny dok{SkupinaPrvehoPovinnehoDokumentu + i}";
                    }
                    zakazkaZO.PovinneDokumenty?.Add(bi);//pridam dokument do kolekcie
                }

                for (short i = 0; i < PocetPriloh; i++)  //vytvorim prazdne Prilohy
                {
                    BaseItem bi = new BaseItem();
                    bi.Skupina = (short)(SkupinaPrvejPrilohy + i);
                    bi.NazovDokumentu = $"Priloha{((SkupinaPrvejPrilohy + i) % SkupinaPrvejPrilohy) + 1:00}"; //Priloha01, Priloha02,...
                    zakazkaZO.Prilohy?.Add(bi);
                }
            }
            return zakazkaZO;
        }//ConvertZakazka_To_ZakazkaZO(Zakazka zakazka)


        /// <summary>
        /// Typ instancie ZakazkaZO skonveruje do NOVEJ instancie Typu Zakazka;
        /// Ak sa udaje  zadane uzivatelom maju INSERTOVAT do databazy;
        /// </summary>
        /// <param name="myZakZO">Udaj editovany uzivatelom</param>
        /// <returns>Udaj ktory sa ma zapisat do databazy</returns>
        public  Zakazka ConvertZakazkaZO_To_NewZakazka(ref ZakazkaZO myZakZO)  //Ak sa vytvara nova instancia typu Zakazka pre INSERT do DB!!!
        {
            if (myZakZO == null)
            {
                throw new ArgumentNullException($"Chyba-ConvertZakazkaZO_To_Zakazka: Neplatný údaj pre vstupnú zákazku!");
            }
            Zakazka newZakazka = new Zakazka();

            newZakazka.ZakazkaTb = myZakZO.ZakazkaTb;
            newZakazka.ZakazkaTg = myZakZO.ZakazkaTg;
            newZakazka.Poznamka = myZakZO.Poznamka;
            newZakazka.Ukoncena = myZakZO.Ukoncena;
            newZakazka.Vin = myZakZO.Vin;
            newZakazka.Cws = myZakZO.Cws;
            newZakazka.CisloProtokolu = myZakZO.CisloProtokolu;

           

            //Vytvorenie instancie Dokument z instancie myZakZO.ZakazkaTGdokument a vlozenie do newZakazka.Dokuments;
            if (!string.IsNullOrEmpty(myZakZO?.ZakazkaTGdokument?.NazovSuboru))//je zadany subor; vytvorit instanciu typu Dokument a instanciu typu Document_Detail;
            {
                Dokument dokument1 = new Dokument();
                dokument1.ZakazkaTg = myZakZO.ZakazkaTg;
                dokument1.NazovDokumentu = myZakZO.ZakazkaTGdokument.NazovDokumentu;
                dokument1.NazovSuboru = myZakZO.ZakazkaTGdokument.NazovSuboru;
                dokument1.Skupina = SkupinaZakazkaTGDokument;
                dokument1.Poznamka = null;// "*";

                DokumentDetail dokDetail1 = new DokumentDetail();
                dokDetail1.Skupina = SkupinaZakazkaTGDokument;

                if (myZakZO.ZakazkaTGdokument.FileContent != null)
                {
                    dokDetail1.DokumentContent = new byte[myZakZO.ZakazkaTGdokument.FileContent.Length];
                    myZakZO.ZakazkaTGdokument.FileContent.CopyTo(dokDetail1.DokumentContent, 0);
                }

                dokument1.DokumentDetails.Add(dokDetail1);
                newZakazka.Dokuments.Add(dokument1);
            }

            //Vytvorenie instancie Dokument z instancie myZakZO.ZakazkaTBdokument  a vlozenie do newZakazka.Dokuments;
            if (!string.IsNullOrEmpty(myZakZO?.ZakazkaTBdokument?.NazovSuboru))//je zadany subor; vytvorit instanciu typu Dokument a instanciu typu Document_Detail;
            {
                Dokument dokument1 = new Dokument();
                dokument1.ZakazkaTg = myZakZO.ZakazkaTb ?? "0";  //ak nie je zadana ZakazkaTb zapisem tam "0", ale ak existuje ZakazkaTBdokument.NazovSuboru musi byt zadane aj ZakazkaTb
                dokument1.NazovDokumentu = myZakZO.ZakazkaTBdokument.NazovDokumentu;
                dokument1.NazovSuboru = myZakZO.ZakazkaTBdokument.NazovSuboru;
                dokument1.Skupina = SkupinaZakazkaTBDokument;
                dokument1.Poznamka = null; // string.Empty;

                DokumentDetail dokDetail1 = new DokumentDetail();
                dokDetail1.Skupina = SkupinaZakazkaTBDokument;
                //dokDetail1.DokumentContent = System.IO.File.ReadAllBytes(myZakZO.ZakazkaTBdokument.FilePath);
                if(myZakZO.ZakazkaTBdokument.FileContent != null)
                {
                    dokDetail1.DokumentContent = new byte[myZakZO.ZakazkaTBdokument.FileContent.Length];
                    myZakZO.ZakazkaTBdokument.FileContent?.CopyTo(dokDetail1.DokumentContent, 0);
                }
                dokument1.DokumentDetails.Add(dokDetail1);
                newZakazka.Dokuments.Add(dokument1);
            }

            //Zadane Povinne dokumenty skopirujeme do newZakazka.Dokuments;
            bool? documentExist = myZakZO?.PovinneDokumenty?.Any(p => !string.IsNullOrEmpty(p.NazovDokumentu));
            if (documentExist.HasValue && documentExist.Value)//je zadany aspon jeden subor
            {
                var povinneDokumenty = myZakZO?.PovinneDokumenty?.Where(p => !string.IsNullOrEmpty(p.NazovSuboru)).ToList();
                if( povinneDokumenty != null)
                {
                    foreach (var povinnyDokument in povinneDokumenty)
                    {
                        Dokument dokument1 = new Dokument();
                        dokument1.ZakazkaTg = myZakZO?.ZakazkaTg ?? "";
                        dokument1.NazovDokumentu = povinnyDokument.NazovDokumentu;
                        dokument1.NazovSuboru = povinnyDokument.NazovSuboru;
                        dokument1.Skupina = povinnyDokument.Skupina;
                        dokument1.Poznamka = povinnyDokument.Poznamka;

                        DokumentDetail dokDetail1 = new DokumentDetail();
                        
                        dokDetail1.Skupina = povinnyDokument.Skupina;
                        if( povinnyDokument.FileContent != null)  //FileContent pole bytov, obsahuje obrazok z klienta;
                        {
                            dokDetail1.DokumentContent = new byte[povinnyDokument.FileContent.Length];
                            povinnyDokument.FileContent?.CopyTo(dokDetail1.DokumentContent, 0);
                        }

                        dokument1.DokumentDetails.Add(dokDetail1);
                        newZakazka.Dokuments.Add(dokument1);
                    }
                }
            }
          
            //Zadane Prilohy skopirujeme do newZakazka.Dokuments;
            documentExist = myZakZO?.Prilohy?.Any(p => !string.IsNullOrEmpty(p.NazovSuboru));
            if (documentExist.HasValue && documentExist.Value)//je zadany aspon jeden subor
            {
                var prilohy = myZakZO?.Prilohy?.Where(p => !string.IsNullOrEmpty(p.NazovSuboru)).ToList();
                if (prilohy != null)
                {
                    foreach (var priloha in prilohy)
                    {
                        Dokument dokument1 = new Dokument();
                        dokument1.ZakazkaTg = myZakZO?.ZakazkaTg ?? "0";
                        dokument1.NazovDokumentu = priloha.NazovDokumentu;
                        dokument1.NazovSuboru = priloha.NazovSuboru;
                        dokument1.Skupina = priloha.Skupina;
                        dokument1.Poznamka = priloha.Poznamka;

                        DokumentDetail dokDetail1 = new DokumentDetail();
                        dokDetail1.Skupina = priloha.Skupina;

                        if( priloha.FileContent != null) //FileContent pole bytov, obsahuje obrazok z klienta;
                        {
                            dokDetail1.DokumentContent = new byte[priloha.FileContent.Length];
                            priloha.FileContent?.CopyTo(dokDetail1.DokumentContent, 0);
                        }
                        dokument1.DokumentDetails.Add(dokDetail1);
                        newZakazka.Dokuments.Add(dokument1);
                    }
                }
            }
            return newZakazka;
        }//ConvertZakazkaZO_To_Zakazka(ZakazkaZO myZakZO)

        /*UPDATE instancie typu Zakazka
         * Z  DB bola nacitana instancia typu Zakazka;
         * Z nej som vytvoril instanciu typu ZakazkaZO (je to viewModel pre zobrazenie vo View )
         * Instancia typu ZakazkaZO je zobrazena vo view a uzivatel mohol zmenit udaje...a stlacil "Ulozit zmeny".
         * Teraz musim preniest zmeny z upravenej instancie typu ZakazkaZO do povodnej instancie typu Zakazka a volat _context.SaveChanges();
         * 
         * 
         * Button "Clear" vymaze dokument pre danu skupinu a refresne stranku
         * Ak ZakazkaZO dokument.NazovSuboru je zadany a DoFormFile =null, znamena ze sa subor NEZMENIL!!!
         *                                   nie je zadany DoFormFile != null, pridany dokument !!!
         * 
         * 
         * Instancia myZakZO vzikla z instancie zakazkaDB, pri editovani myZakZO sa mohli zmenit obsahy instancii dokumentov, mohol pribudnut dokument do myZakZO;
         */
        public void ConvertZakazkaZO_To_Zakazka(ref ZakazkaZO myZakZO, ref Zakazka zakazkaDB)  //ked sa robi UPDATE zaznamu z DB!!!!
        {
            if (myZakZO == null)
            {
                throw new ArgumentNullException($"Chyba-ConvertZakazkaZO_To_Zakazka: Neplatný údaj pre vstupnú zákazku!");
            }
            if(zakazkaDB == null)
            {
                throw new ArgumentNullException($"Chyba-ConvertZakazkaZO_To_Zakazka: Neplatný údaj pre výstupnú zákazku!");
            }

            //zakazkaDB.ZakazkaTg = myZakZO.ZakazkaTg;   //ZakazkaTg sa nemeni!!!
            zakazkaDB.ZakazkaTb = myZakZO.ZakazkaTb;

            zakazkaDB.Poznamka = myZakZO?.Poznamka?.Trim();
            zakazkaDB.Ukoncena = myZakZO?.Ukoncena;
            zakazkaDB.Vin = myZakZO?.Vin?.Trim();
            zakazkaDB.Cws = myZakZO?.Cws?.Trim();
            zakazkaDB.CisloProtokolu = myZakZO?.CisloProtokolu?.Trim();

            #region == Dokumenty  pre Skupina=1 a Skupina=2 ===

            Dokument? zakazkaTGdokumentDB = zakazkaDB.Dokuments.FirstOrDefault(d => d.Skupina == SkupinaZakazkaTGDokument);
            if (zakazkaTGdokumentDB != null)//zakazkaDB mala uz vytvoreny zakazkaTGdokument, 
            {
                zakazkaTGdokumentDB.Poznamka = myZakZO?.ZakazkaTGdokument?.Poznamka?.Trim();
                if(myZakZO?.ZakazkaTGdokument?.DokFormFile != null)  //nastala zmena obrazku
                {
                    using (var ms = new MemoryStream())
                    {
                        zakazkaTGdokumentDB.NazovSuboru = myZakZO?.ZakazkaTGdokument.NazovSuboru?.Trim();  //ZakazkaTGdokument.DokFormFile.FileName;
                        myZakZO?.ZakazkaTGdokument.DokFormFile.CopyTo(ms);

                        DokumentDetail? detailDB = zakazkaTGdokumentDB.DokumentDetails.FirstOrDefault(d => d.Skupina == SkupinaZakazkaTGDokument);
                        if (detailDB != null && detailDB.DokumentContent != null)
                        {
                            detailDB.DokumentContent = ms.ToArray();
                        }
                    }
                }
             /*   */
            }
            else//zakazkaDB ak bola uz ulozena nemusi mat este vytvoreny ZakazkaTGdokument!!  Dokument pre Skupina=1
            {
                if(myZakZO?.ZakazkaTGdokument?.DokFormFile != null) //pre myZakZO bol zadany obrazok pre ZakazkaTGdokument;
                {
                    Dokument dokumentDB = new Dokument();
                    dokumentDB.ZakazkaTg = myZakZO.ZakazkaTg;
                    dokumentDB.NazovDokumentu = myZakZO?.ZakazkaTGdokument?.NazovDokumentu;
                    dokumentDB.NazovSuboru = myZakZO?.ZakazkaTGdokument.NazovSuboru;
                    dokumentDB.Skupina = SkupinaZakazkaTGDokument;
  
                    DokumentDetail detailDB = new DokumentDetail(); //pre dokument1 vytvorime DokumentDetail
                    detailDB.Skupina = dokumentDB?.Skupina;

                    using (var ms = new MemoryStream())
                    {
                        myZakZO?.ZakazkaTGdokument.DokFormFile.CopyTo(ms);
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
                zakazkaTBdokumentDB.Poznamka = myZakZO?.ZakazkaTBdokument?.Poznamka;

                if (myZakZO?.ZakazkaTBdokument?.DokFormFile != null)  //nastala zmena obrazku
                {
                    using (var ms = new MemoryStream())
                    {
                        zakazkaTBdokumentDB.NazovSuboru = myZakZO.ZakazkaTBdokument.NazovSuboru;
                        myZakZO.ZakazkaTBdokument.DokFormFile.CopyTo(ms);

                        DokumentDetail? detailDB = zakazkaTBdokumentDB.DokumentDetails.FirstOrDefault(d => d.Skupina == SkupinaZakazkaTBDokument);
                        if (detailDB != null && detailDB.DokumentContent != null)
                        {
                            detailDB.DokumentContent = ms.ToArray();
                        }
                    }
                }
            }
            else//zakazkaDB este nemala vytvoreny ZakazkaTBdokumentDB pre Skupina=2
            {
                if (myZakZO?.ZakazkaTBdokument?.DokFormFile != null) //pre myZakZO bol zadany obrazok pre ZakazkaTBdokument;
                {
                    Dokument dokumentDB = new Dokument();
                    dokumentDB.ZakazkaTg = myZakZO.ZakazkaTg;
                    dokumentDB.NazovDokumentu = myZakZO.ZakazkaTBdokument.NazovDokumentu;
                    dokumentDB.NazovSuboru = myZakZO.ZakazkaTBdokument.NazovSuboru;
                    dokumentDB.Skupina = SkupinaZakazkaTBDokument;

                    DokumentDetail detailDB = new DokumentDetail(); //pre dokument1 vytvorime DokumentDetail
                    detailDB.Skupina = dokumentDB?.Skupina;

                    using (var ms = new MemoryStream())
                    {
                        myZakZO.ZakazkaTBdokument.DokFormFile.CopyTo(ms);
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

            bool existujuPovinneDokumentyZO = myZakZO?.PovinneDokumenty?.Any(d => !string.IsNullOrEmpty(d.NazovSuboru)) ?? false;
            if ( existujuPovinneDokumentyZO  )
                povinneDokumentyZO = myZakZO?.PovinneDokumenty?.Where(d => !string.IsNullOrEmpty(d.NazovSuboru)).ToList();

            bool existujuPovinneDokumentyDB = zakazkaDB.Dokuments.Any(d => d.Skupina >= SkupinaPrvehoPovinnehoDokumentu && d.Skupina < SkupinaPrvejPrilohy);

            if (existujuPovinneDokumentyDB)
                povinneDokumentyDB = zakazkaDB.Dokuments.Where(d => d.Skupina >= SkupinaPrvehoPovinnehoDokumentu && d.Skupina < SkupinaPrvejPrilohy).ToList();

            if (!existujuPovinneDokumentyDB && existujuPovinneDokumentyZO)//zakazkaDB este nemala vytvorene Povinne dokumenty
            {
                //vytvorim nove dokumenty a vlozim ich do  zakazkaDB.Dokuments.
                if (povinneDokumentyZO != null)
                {
                    foreach (BaseItem pdZO in povinneDokumentyZO)
                    {
                        Dokument dokumentDB = new Dokument();
                        dokumentDB.ZakazkaTg = myZakZO.ZakazkaTg;
                        dokumentDB.NazovDokumentu = pdZO.NazovDokumentu;
                        dokumentDB.NazovSuboru = pdZO.NazovSuboru;
                        dokumentDB.Skupina = pdZO.Skupina;
                        dokumentDB.Poznamka = pdZO.Poznamka;

                        DokumentDetail dokDetailDB = new DokumentDetail();
                        dokDetailDB.Skupina = pdZO.Skupina;
                        dokDetailDB.DokumentContent = new byte[pdZO.FileContent.Length]; 
                        pdZO.FileContent.CopyTo(dokDetailDB.DokumentContent,0);
                        

                        dokumentDB.DokumentDetails.Add(dokDetailDB);
                        zakazkaDB.Dokuments.Add(dokumentDB);
                    }
                }
            }
            else if (existujuPovinneDokumentyDB && existujuPovinneDokumentyZO) //zakazkaDB uz ma nejake Povinne dokumenty; myZakZO ma vzdy 5 Povinnych Dokumentov, ale nemusia mat zadany NazovSuboru
            {
                for (short i = 0; i < PocetPovinnych; i++)
                {
                    bool existujePDZO = myZakZO?.PovinneDokumenty?.Any(d => !string.IsNullOrEmpty(d.NazovSuboru) && d.Skupina == SkupinaPrvehoPovinnehoDokumentu + i) ?? false;
                    bool existujePDDB = zakazkaDB.Dokuments.Any(d => !string.IsNullOrEmpty(d.NazovSuboru) && d.Skupina == SkupinaPrvehoPovinnehoDokumentu + i);


                    if (existujePDDB && existujePDZO) //robi sa update-zmena povinneho dokumentu
                    {
                        Dokument? dokumentDB = zakazkaDB.Dokuments.FirstOrDefault(d => !string.IsNullOrEmpty(d.NazovSuboru) && d.Skupina == SkupinaPrvehoPovinnehoDokumentu + i);
                        BaseItem? pdZO = myZakZO?.PovinneDokumenty?.FirstOrDefault(d => !string.IsNullOrEmpty(d.NazovSuboru) && d.Skupina == SkupinaPrvehoPovinnehoDokumentu + i);
                        if (dokumentDB != null && pdZO != null) //robi sa update-zmena povinneho dokumentu
                        {
                            //dokumentDB.NazovDokumentu sa nemeni
                            //dokumentDB.Skupina sa nemeni
                            dokumentDB.Poznamka = pdZO?.Poznamka;

                            if( pdZO?.DokFormFile != null ) //zmenil sa obrazok
                            {
                                dokumentDB.NazovSuboru = pdZO?.NazovSuboru;
                                DokumentDetail detailDB = dokumentDB.DokumentDetails.FirstOrDefault(d => d.Skupina == dokumentDB.Skupina);
                                if (detailDB != null)
                                {
                                    detailDB.DokumentContent = new byte[pdZO.FileContent.Length];
                                    pdZO.FileContent.CopyTo(detailDB.DokumentContent, 0); 
                                }
                            }
                        }
                    }
                    //v zakazkaDB este nema vytvoreny Povinny dokument kde Skupina=SkupinaPrvehoPovinnehoDokumentu + i
                    //myZakZo ma Povinny dokument kde Skupina=SkupinaPrvehoPovinnehoDokumentu + i
                    //vytvorim novy dokument a pridam ho do zakazkaDB.Dokuments
                    else if (!existujePDDB && existujePDZO)
                    {
                        BaseItem? pdZO = myZakZO?.PovinneDokumenty?.FirstOrDefault(d => !string.IsNullOrEmpty(d.NazovSuboru) && d.Skupina == SkupinaPrvehoPovinnehoDokumentu + i);

                        if (pdZO != null)
                        {
                            Dokument dokumentDB = new Dokument();
                            dokumentDB.ZakazkaTg = myZakZO.ZakazkaTg;
                            dokumentDB.NazovDokumentu = pdZO.NazovDokumentu;
                            dokumentDB.NazovSuboru = pdZO.NazovSuboru;
                            dokumentDB.Skupina = pdZO.Skupina;
                            dokumentDB.Poznamka = pdZO.Poznamka;

                            DokumentDetail detailDB = new DokumentDetail();
                            detailDB.Skupina = pdZO.Skupina;

                            detailDB.DokumentContent = new byte[pdZO.FileContent.Length];
                            pdZO.FileContent.CopyTo(detailDB.DokumentContent, 0);

                            dokumentDB.DokumentDetails.Add(detailDB);
                            zakazkaDB.Dokuments.Add(dokumentDB);
                        }
                    }
                    //v zakazkaDB ma vytvoreny Povinny dokument kde Skupina=SkupinaPrvehoPovinnehoDokumentu + i, ale myZakZO nema zadany subor pre Povinny dokument
                    // v myZakZo.PovinneDokumenty sa nemenil subor kde  Skupina=SkupinaPrvehoPovinnehoDokumentu + i
                    else if (existujePDDB &&  !existujePDZO)
                    {
                        //Dokument? dokumentDB = zakazkaDB.Dokuments.FirstOrDefault(d => !string.IsNullOrEmpty(d.NazovSuboru) && d.Skupina == SkupinaPrvehoPovinnehoDokumentu + i);
                        //if (dokumentDB != null)
                        //{
                        //    zakazkaDB.Dokuments.Remove(dokumentDB);
                        //}
                    }
                }
            }//if(!existujuPovinneDokumentyDB....)

            #endregion == Povinne dokumenty ==

            #region == Prilohy ==

            List<BaseItem>? prilohyZO = null;
            List<Dokument>? prilohyDB = null;

            bool existujuPrilohyZO = myZakZO?.Prilohy?.Any(d => !string.IsNullOrEmpty(d.NazovSuboru)) ?? false;

            if (existujuPrilohyZO) //ak existuje dokument pre prilohu, je nastavene pole pre obrazok
                prilohyZO = myZakZO?.Prilohy?.Where(d => !string.IsNullOrEmpty(d.NazovSuboru)).ToList();

            bool existujuPrilohyDB = zakazkaDB.Dokuments.Any(d => d.Skupina >= SkupinaPrvejPrilohy);

            if (existujuPrilohyDB)
                prilohyDB = zakazkaDB.Dokuments.Where(d => d.Skupina >= SkupinaPrvejPrilohy).ToList();

            if (!existujuPrilohyDB && existujuPrilohyZO)//zakazkaDB este nemala vytvorene Prilohy
            {
                //vytvorim nove dokumenty pre zakazkaDB a pridam ich do  zakazkaDB.Dokuments.
                if ( (prilohyDB != null) && (prilohyZO != null) )
                {
                    foreach (var dokumentZO in prilohyZO)
                    {
                        Dokument dokumentDB = new Dokument();
                        dokumentDB.ZakazkaTg = myZakZO.ZakazkaTg;
                        dokumentDB.NazovDokumentu = dokumentZO.NazovDokumentu;
                        dokumentDB.NazovSuboru = dokumentZO.NazovSuboru;
                        dokumentDB.Skupina = dokumentZO.Skupina;
                        dokumentDB.Poznamka = dokumentZO.Poznamka;

                        DokumentDetail dokDetailDB = new DokumentDetail();
                        dokDetailDB.Skupina = dokumentZO.Skupina;
                        if (dokumentZO.FileContent != null)
                        {
                            dokDetailDB.DokumentContent = new byte[dokumentZO.FileContent.Length];
                            dokumentZO.FileContent.CopyTo(dokDetailDB.DokumentContent, 0);

                            dokumentDB.DokumentDetails.Add(dokDetailDB);
                            zakazkaDB.Dokuments.Add(dokumentDB);
                        }
                    }
                }
            }
            else if (existujuPrilohyDB && existujuPrilohyZO)//zakazkaDB uz ma nejake Prilohy a zakazkaZO ma nejake prilohy;
            {

                for (short i = 0; i < PocetPriloh; i++)
                {
                    bool existujePRZO = myZakZO?.Prilohy?.Any(d => !string.IsNullOrEmpty(d.NazovSuboru) && d.Skupina == SkupinaPrvejPrilohy + i) ?? false;
                    bool existujePRDB = zakazkaDB.Dokuments.Any(d => !string.IsNullOrEmpty(d.NazovSuboru) && d.Skupina == SkupinaPrvejPrilohy + i);
                    if (existujePRDB && existujePRZO)
                    {
                        Dokument? dokumentDB = zakazkaDB.Dokuments.FirstOrDefault(d => !string.IsNullOrEmpty(d.NazovSuboru) && d.Skupina == SkupinaPrvejPrilohy + i);
                        BaseItem? dokumentZO = myZakZO?.Prilohy?.FirstOrDefault(d => !string.IsNullOrEmpty(d.NazovSuboru) && d.Skupina == SkupinaPrvejPrilohy + i);

                        if ((dokumentDB != null)  && (dokumentZO != null))
                        {
                            if (dokumentDB.NazovSuboru != dokumentZO.NazovSuboru)//zmenil sa subor pre obrazok
                            {
                                dokumentDB.NazovSuboru = dokumentZO.NazovSuboru; 
                           
                                dokumentDB.Poznamka = dokumentZO.Poznamka;
                                DokumentDetail? detailDB = dokumentDB.DokumentDetails?.FirstOrDefault(d => d.Skupina == dokumentDB.Skupina);
                                if (detailDB != null &&  dokumentZO.FileContent != null)
                                {
                                    detailDB.DokumentContent = new byte[dokumentZO.FileContent.Length];
                                    dokumentZO.FileContent.CopyTo(detailDB.DokumentContent, 0);
                                }
                            }
                        }
                        
                    }
                    //v zakazkaDB este nema vytvoreny Povinny dokument kde Skupina=SkupinaPrvehoPovinnehoDokumentu + i
                    //myZakZo ma Povinny dokument kde Skupina=SkupinaPrvehoPovinnehoDokumentu + i
                    //vytvorim novy dokument a pridam ho do zakazkaDB.Dokuments
                    else if (!existujePRDB &&  existujePRZO)
                    {
                        BaseItem? dokumentZO = myZakZO?.Prilohy?.FirstOrDefault(d => !string.IsNullOrEmpty(d.NazovSuboru) && d.Skupina == SkupinaPrvejPrilohy + i);

                        if (dokumentZO != null)
                        {
                            Dokument dokumentDB = new Dokument();
                            dokumentDB.ZakazkaTg = myZakZO.ZakazkaTg;
                            dokumentDB.NazovDokumentu = dokumentZO.NazovDokumentu;
                            dokumentDB.NazovSuboru = dokumentZO.NazovSuboru;
                            dokumentDB.Skupina = dokumentZO.Skupina;
                            dokumentDB.Poznamka = dokumentZO.Poznamka;

                            DokumentDetail dokDetailDB = new DokumentDetail();
                            dokDetailDB.Skupina = dokumentZO.Skupina;
                            if ( dokumentZO.FileContent != null)
                            {
                                dokDetailDB.DokumentContent = new byte[dokumentZO.FileContent.Length];
                                dokumentZO.FileContent.CopyTo(dokDetailDB.DokumentContent, 0);
                            }

                            dokumentDB.DokumentDetails.Add(dokDetailDB);
                            zakazkaDB.Dokuments.Add(dokumentDB);
                        }
                    }
                    //v zakazkaDB ma vytvorenu Prilohu kde Skupina=SkupinaPrvejPrilohy + i, ale myZakZO nema zadany subor pre Prilohu
                    // v myZakZO.Prilohy bol vymazany subor kde  Skupina=SkupinaPrvejPrilohy + i
                    else if (existujePRDB && !existujePRZO)
                    {
                        Dokument? dokumentDB = zakazkaDB.Dokuments.FirstOrDefault(d => !string.IsNullOrEmpty(d.NazovSuboru) && d.Skupina == SkupinaPrvejPrilohy + i);
                        if(dokumentDB != null)
                            zakazkaDB.Dokuments.Remove(dokumentDB);
                    }
                }// for (short i = 0; i < PocetPriloh; i++)
            }//if(!existujuPovinneDokumentyDB....)

            #endregion == Prilohy ==

        }//ConvertZakazkaZO_To_Zakazka(ZakazkaZO myZakZO, ref Zakazka zakazkaDB)


        /// <summary>
        /// Vytvori instanciu ktora obsahuje vsetky vnorene udaje, ale udaje este nie su nastavene;
        /// Povinne dokumenty obsahuju len NazovDokumentu a cislo skupiny;
        /// Prilohy  obsahuju len NazovDokumentu  a cislo skupiny;
        /// </summary>
        /// <returns></returns>
        public ZakazkaZO VytvorPrazdnuZakazkuZO()
        {
            ZakazkaZO zakazkaZO = new ZakazkaZO();

            VytvorPovinneDokumenty();
            VytvorPrilohy();

            return zakazkaZO;

            int VytvorPovinneDokumenty()
            {
                for (short i = 0; i < PocetPovinnych; i++)
                {
                    zakazkaZO.PovinneDokumenty?.Add(new BaseItem() { Skupina = (short)(SkupinaPrvehoPovinnehoDokumentu + i), NazovDokumentu = NazvyPovinnychDokumentov[i], Poznamka = $"Poznamka ABCD{i + 1:000}" });
                }
                return zakazkaZO.PovinneDokumenty?.Count ?? 0;
            }

            int VytvorPrilohy()
            {
                for (short i = 0; i < PocetPriloh; i++)
                {
                    zakazkaZO.Prilohy?.Add(new BaseItem() { Skupina = (short)(SkupinaPrvejPrilohy + i), NazovDokumentu = $"Priloha{(SkupinaPrvejPrilohy + i + 1) % SkupinaPrvejPrilohy:00}", Poznamka = $"POZNAMKA Priloha ABCD{i + 1:000}" });
                }
                return zakazkaZO.Prilohy?.Count ?? 0;
            }
        }
    }
}
