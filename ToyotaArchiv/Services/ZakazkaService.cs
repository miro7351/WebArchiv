using ToyotaArchiv.Domain;
using PA.TOYOTA.DB;
using ToyotaArchiv.Infrastructure;

namespace ToyotaArchiv.Services
{
    //MH: 12.04.2022
    /*
     * Typ ZakazkaZO je ViewModel pre typ Zakazka;
     * Modul obsahuje funcie na transformaciu typu ZakazkaZO <-> typ Zakazka
     */ 
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
        static short PocetPovinnych => 5;//Pocet sa nemeni v programe
        static short PocetPriloh => 5;   //Pocet sa moze zvascovat, len na zaciatku sa vytvori 5 priloh
        static short PocetPriloh1 => 1;  //Pocet sa moze zvascovat, len na zaciatku sa vytvori 1 priloha

        /// <summary>
        /// Typ instancie Zakazka (z databazy) skonveruje do vytvorenej instancie typu ZakazkaZO (pre zobrazenie pre uzivatela);
        /// Ak sa udaje nacitane z databazy maju len zobrazit uzivatelovi;
        /// </summary>
        /// <param name="zakazkaDB">udaj nacitany z databazy</param>
        /// <returns>ZakazkaZO typ pre editaciu alebo zobrazenie</returns>
        public ZakazkaZO ConvertZakazka_To_ZakazkaZO(ref Zakazka zakazkaDB)
        {
            if( zakazkaDB == null)
            {
                throw new ArgumentNullException($"ConvertZakazka_To_ZakazkaZO - Chyba: zle zadaný vstupný parameter!");
            }

            ZakazkaZO zakazkaZO = new ZakazkaZO();
            //zakazkaZO.ZakazkaTGdokument neobsahuje obrazok, tj. ZakazkaTGdokument.FileContent=null
            //obrazok sa na poziadanie vyberie z DB!!!

            zakazkaZO.ZakazkaTg = zakazkaDB.ZakazkaTg.Trim();
            zakazkaZO.ZakazkaTb = zakazkaDB.ZakazkaTb?.Trim();
            zakazkaZO.Poznamka = zakazkaDB?.Poznamka?.Trim();
            zakazkaZO.Ukoncena = zakazkaDB?.Ukoncena;
            zakazkaZO.Vin = zakazkaDB?.Vin?.Trim();
            zakazkaZO.Cws = zakazkaDB?.Cws?.Trim();
            zakazkaZO.CisloProtokolu = zakazkaDB?.CisloProtokolu?.Trim();
            zakazkaZO.Vytvoril = zakazkaDB?.Vytvoril?.Trim();
            zakazkaZO.Vytvorene = zakazkaDB?.Vytvorene;    //DateTime
            zakazkaZO.Zmenil = zakazkaDB?.Zmenil?.Trim(); 
            zakazkaZO.Zmenene = zakazkaDB?.Zmenene;    //DateTime

            //var d1 = zakazka.Dokuments.Where(d => d.Skupina == 1).FirstOrDefault();
            //var detaily = d1.DokumentDetails.FirstOrDefault(d => d.Skupina == 1);
            int pd = zakazkaDB?.Dokuments?.Count ?? 0;
            bool existujuDokumenty = zakazkaDB?.Dokuments.Any(d => !string.IsNullOrEmpty(d.NazovDokumentu)) ?? false;

            if (existujuDokumenty) //existuju zaznamy pre dokumenty
            {
                //Test ci existuje dokument pre zakazkaZO.ZakazkaTGdokument
                if (zakazkaDB.Dokuments.Any(d => d.Skupina == SkupinaZakazkaTGDokument))//vytvorenie zakazkaZO.ZakazkaTGdokument
                {
                    Dokument? dokDB = zakazkaDB.Dokuments.FirstOrDefault(d => d.Skupina == SkupinaZakazkaTGDokument);
                    DokumentDetail? detail = dokDB?.DokumentDetails.FirstOrDefault(d => d.Skupina == SkupinaZakazkaTGDokument);

                    zakazkaZO.ZakazkaTGdokument.NazovDokumentu = dokDB?.NazovDokumentu?.Trim();
                    zakazkaZO.ZakazkaTGdokument.NazovSuboru    = dokDB?.NazovSuboru?.Trim();

                    zakazkaZO.ZakazkaTGdokument.FileContent = null;

                    zakazkaZO.ZakazkaTGdokument.Skupina   = dokDB?.Skupina;//short
                    zakazkaZO.ZakazkaTGdokument.Poznamka  = dokDB?.Poznamka?.Trim();
                }

                //Test ci existuje dokument pre zakazkaZO.ZakazkaTGdokument
                if (zakazkaDB.Dokuments.Any(d => d.Skupina == SkupinaZakazkaTBDokument))//vytvorenie zakazkaZO.ZakazkaTBdokument
                {
                    Dokument? dokDB = zakazkaDB.Dokuments.FirstOrDefault(d => d.Skupina == SkupinaZakazkaTBDokument);
                   // DokumentDetail detail = dok?.DokumentDetails.FirstOrDefault(d => d.Skupina == SkupinaZakazkaTBDokument);

                    zakazkaZO.ZakazkaTBdokument.NazovDokumentu = dokDB?.NazovDokumentu?.Trim();
                    zakazkaZO.ZakazkaTBdokument.NazovSuboru = dokDB?.NazovSuboru?.Trim();

                    zakazkaZO.ZakazkaTGdokument.FileContent = null;
                   
                    zakazkaZO.ZakazkaTBdokument.Skupina = dokDB?.Skupina;//short
                    zakazkaZO.ZakazkaTBdokument.Poznamka = dokDB?.Poznamka?.Trim();
                }

                #region ==Povinne dokumenty ==

                if (zakazkaDB.Dokuments.Any(d => d.Skupina >= SkupinaPrvehoPovinnehoDokumentu && d.Skupina < SkupinaPrvejPrilohy))
                {
                    //pocet zaznamov pre povinne dokumety
                    int pz = zakazkaDB.Dokuments.Count(d => d.Skupina >= SkupinaPrvehoPovinnehoDokumentu && d.Skupina < SkupinaPrvejPrilohy);

                    if (pz > 0)  //existuje nejaky povinny dokument
                    {
                        for (short i = 0; i < PocetPovinnych; i++)
                        {
                            Dokument? dokDB = zakazkaDB.Dokuments.FirstOrDefault(d => d.Skupina == (SkupinaPrvehoPovinnehoDokumentu + i));
                            BaseItem bi = new BaseItem();
                            if (dokDB != null) //existuje povinny dokument, kde Skupina= (SkupinaPrvehoPovinnehoDokumentu+i)
                            {
                                bi.Skupina = dokDB.Skupina; //short
                                bi.NazovDokumentu = dokDB.NazovDokumentu?.Trim();  //
                                bi.Poznamka = dokDB.Poznamka?.Trim() ?? "*";
                                bi.NazovSuboru = dokDB.NazovSuboru?.Trim();
                            }
                            else //neexistuje povinny dokument, vytvorim prazdny dokument a nastavim  len jeho property NazovDokumentu a Skupina
                            {
                                bi.Skupina = (short)(SkupinaPrvehoPovinnehoDokumentu + i);//vytvorim jeho skupinu
                                bi.Poznamka = "*";
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
                
                else  //neexistuju este povinne dokumenty, vytvorim prazdne povinne dokumenty
                {
                    for (short i = 0; i < PocetPovinnych; i++)
                    {
                        BaseItem bi = new BaseItem();
                        bi.Skupina = (short)(SkupinaPrvehoPovinnehoDokumentu + i);//zapisem jeho skupinu
                        bi.Poznamka = "*";
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
               
                #endregion ==Povinne dokumenty ==

                #region == Prilohy ==
                if (zakazkaDB.Dokuments.Any(d => d.Skupina >= SkupinaPrvejPrilohy))//Prilohy
                {
                    //pocet zaznamov pre prilohy
                    int pz = zakazkaDB.Dokuments.Count(d => d.Skupina >= SkupinaPrvejPrilohy);

                    if (pz > 0)  //existuju prilohy
                    {
                        var dokumentyPrilohyDB = zakazkaDB.Dokuments.Where(d => d.Skupina >= SkupinaPrvejPrilohy).ToList();
                        foreach (var dokDB in dokumentyPrilohyDB)
                        {
                            BaseItem bi = new BaseItem();
                            bi.Skupina = dokDB.Skupina;
                            bi.NazovDokumentu = dokDB.NazovDokumentu?.Trim();  //
                            
                            bi.Poznamka = dokDB.Poznamka?.Trim() ?? "*";
                            bi.NazovSuboru = dokDB.NazovSuboru?.Trim();
                            zakazkaZO.Prilohy?.Add(bi);
                        }//for

                        //VYtvori sa len tolko Priloh kolko mala zakazka
                        //doplnenie  poctu  priloh do 'PocetPriloh', nastavim Skupinu a NazovDokumentu PrilohaXX
                        //if (PocetPriloh > pz)
                        //{
                        //    for (short i = 0; i < PocetPriloh - pz; i++)
                        //    {
                        //        BaseItem bi = new BaseItem();
                        //        bi.Skupina = (short)(SkupinaPrvejPrilohy + pz + i);
                        //        bi.NazovDokumentu = $"Priloha{(SkupinaPrvejPrilohy + pz + i+1) % SkupinaPrvejPrilohy:00}";
                        //        zakazkaZO.Prilohy?.Add(bi);
                        //    }
                        //}
                    }
                }
                else //neexistuju prilohy, vytvorim PocetPriloh prazdnych priloh
                {
                    for (short i = 0; i < PocetPriloh1; i++)
                    {
                        BaseItem bi = new BaseItem();
                        bi.Skupina = (short)(SkupinaPrvejPrilohy + i);
                        bi.NazovDokumentu = $"Priloha{((SkupinaPrvejPrilohy + i) % SkupinaPrvejPrilohy) + 1:00}"; //Priloha01, Priloha02,...
                        bi.Poznamka = "*";
                        zakazkaZO.Prilohy?.Add(bi);
                    }
                }
               
                #endregion == Prilohy ==
            }//if( zakazka.Dokuments.Any(d => !string.IsNullOrEmpty(d.NazovDokumentu)) ) //existuju zaznamy pre dokumety
            else
            {  // zakazku nema este nijake dokumenty

                for (short i = 0; i < PocetPovinnych; i++)//vytvorim Povinne prazdne dokumenty
                {
                    BaseItem bi = new BaseItem();
                    bi.Skupina = (short)(SkupinaPrvehoPovinnehoDokumentu + i);//zapisem jeho skupinu
                    bi.Poznamka = "*";
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

                for (short i = 0; i < PocetPriloh1; i++)  //vytvorim prazdne Prilohy
                {
                    BaseItem bi = new BaseItem();
                    bi.Skupina = (short)(SkupinaPrvejPrilohy + i);
                    bi.Poznamka = "*";
                    bi.NazovDokumentu = $"Priloha{((SkupinaPrvejPrilohy + i) % SkupinaPrvejPrilohy) + 1:00}"; //Priloha01, Priloha02,...
                    zakazkaZO.Prilohy?.Add(bi);
                }
            }
            return zakazkaZO;
        }//ConvertZakazka_To_ZakazkaZO(ref Zakazka zakazka)


        /// <summary>
        /// Typ instancie ZakazkaZO skonveruje do NOVEJ instancie Typu Zakazka;
        /// Ak sa udaje  zadane uzivatelom maju INSERTOVAT do databazy;
        /// </summary>
        /// <param name="zakazkaZO">Instancia editovana uzivatelom</param>
        /// <returns>Instancia, ktora sa ma zapisat do databazy</returns>
        public  Zakazka ConvertZakazkaZO_To_NewZakazka(ref ZakazkaZO zakazkaZO)  //Ak sa vytvara nova instancia typu Zakazka pre INSERT do DB!!!
        {
            if (zakazkaZO == null)
            {
                throw new ArgumentNullException($"Chyba-ConvertZakazkaZO_To_Zakazka: Neplatný údaj pre vstupnú zákazku!");
            }
            Zakazka newZakazkaDB = new Zakazka();

            newZakazkaDB.ZakazkaTb = zakazkaZO.ZakazkaTb;
            newZakazkaDB.ZakazkaTg = zakazkaZO.ZakazkaTg;
            newZakazkaDB.Poznamka = zakazkaZO.Poznamka;
            newZakazkaDB.Ukoncena = zakazkaZO.Ukoncena;
            newZakazkaDB.Vin = zakazkaZO.Vin;
            newZakazkaDB.Cws = zakazkaZO.Cws;
            newZakazkaDB.CisloProtokolu = zakazkaZO.CisloProtokolu;


            //Vytvorenie instancie Dokument z instancie zakazkaZO.ZakazkaTGdokument a vlozenie do newZakazka.Dokuments;
            if (!string.IsNullOrEmpty(zakazkaZO?.ZakazkaTGdokument?.NazovSuboru))//je zadany subor pre ZakazkaTGdokument; vytvorit instanciu typu Dokument a instanciu typu Document_Detail;
            {
                Dokument dokumentDB = new Dokument();
                dokumentDB.ZakazkaTg = zakazkaZO.ZakazkaTg;
                dokumentDB.NazovDokumentu = zakazkaZO.ZakazkaTGdokument.NazovDokumentu;
                dokumentDB.NazovSuboru = zakazkaZO.ZakazkaTGdokument.NazovSuboru;
                dokumentDB.Skupina = SkupinaZakazkaTGDokument;
                dokumentDB.Poznamka = null;// "*";

                DokumentDetail detailDB = new DokumentDetail();
                detailDB.Skupina = SkupinaZakazkaTGDokument;

                if (zakazkaZO.ZakazkaTGdokument.FileContent != null)
                {
                    detailDB.DokumentContent = new byte[zakazkaZO.ZakazkaTGdokument.FileContent.Length];
                    zakazkaZO.ZakazkaTGdokument.FileContent.CopyTo(detailDB.DokumentContent, 0);
                }

                dokumentDB.DokumentDetails.Add(detailDB);
                newZakazkaDB.Dokuments.Add(dokumentDB);
            }

            //Vytvorenie instancie Dokument z instancie myZakZO.ZakazkaTBdokument  a vlozenie do newZakazka.Dokuments;
            if (!string.IsNullOrEmpty(zakazkaZO?.ZakazkaTBdokument?.NazovSuboru))//je zadany subor pre ZakazkaTBdokument; vytvorit instanciu typu Dokument a instanciu typu Document_Detail;
            {
                Dokument dokumentDB = new Dokument();
                dokumentDB.ZakazkaTg = zakazkaZO.ZakazkaTb ?? "0";  //ak nie je zadana ZakazkaTb zapisem tam "0", ale ak existuje ZakazkaTBdokument.NazovSuboru musi byt zadane aj ZakazkaTb
                dokumentDB.NazovDokumentu = zakazkaZO.ZakazkaTBdokument.NazovDokumentu;
                dokumentDB.NazovSuboru = zakazkaZO.ZakazkaTBdokument.NazovSuboru;
                dokumentDB.Skupina = SkupinaZakazkaTBDokument;
                dokumentDB.Poznamka = null; // string.Empty;

                DokumentDetail detailDB = new DokumentDetail();
                detailDB.Skupina = SkupinaZakazkaTBDokument;
                //dokDetail1.DokumentContent = System.IO.File.ReadAllBytes(myZakZO.ZakazkaTBdokument.FilePath);
                if(zakazkaZO.ZakazkaTBdokument.FileContent != null)
                {
                    detailDB.DokumentContent = new byte[zakazkaZO.ZakazkaTBdokument.FileContent.Length];
                    zakazkaZO.ZakazkaTBdokument.FileContent?.CopyTo(detailDB.DokumentContent, 0);
                }
                dokumentDB.DokumentDetails.Add(detailDB);
                newZakazkaDB.Dokuments.Add(dokumentDB);
            }

            //Zadane Povinne dokumenty skopirujeme do newZakazka.Dokuments;
            bool documentExist = zakazkaZO?.PovinneDokumenty?.Any(p => !string.IsNullOrEmpty(p.NazovDokumentu)) ?? false;
            if (documentExist)//je zadany aspon jeden subor
            {
                var povinneDokumentyZO = zakazkaZO?.PovinneDokumenty?.Where(p => !string.IsNullOrEmpty(p.NazovSuboru)).ToList();
                if( povinneDokumentyZO != null)
                {
                    if(povinneDokumentyZO.Count == PocetPovinnych)
                    {
                        newZakazkaDB.Ukoncena = "A";
                    }
                    else
                    {
                        newZakazkaDB.Ukoncena = "N";  
                    }
                    foreach (BaseItem povinnyDokumentZO in povinneDokumentyZO)
                    {
                        Dokument dokumentDB = new Dokument();
                        dokumentDB.ZakazkaTg = zakazkaZO?.ZakazkaTg ?? "";
                        dokumentDB.NazovDokumentu = povinnyDokumentZO.NazovDokumentu;
                        dokumentDB.NazovSuboru = povinnyDokumentZO.NazovSuboru;
                        dokumentDB.Skupina = povinnyDokumentZO.Skupina;
                        dokumentDB.Poznamka = povinnyDokumentZO.Poznamka;

                        DokumentDetail detailDB = new DokumentDetail();
                        
                        detailDB.Skupina = povinnyDokumentZO.Skupina;
                        if( povinnyDokumentZO.FileContent != null)  //FileContent pole bytov, obsahuje obrazok z klienta;
                        {
                            detailDB.DokumentContent = new byte[povinnyDokumentZO.FileContent.Length];
                            povinnyDokumentZO.FileContent?.CopyTo(detailDB.DokumentContent, 0);
                        }

                        dokumentDB.DokumentDetails.Add(detailDB);
                        newZakazkaDB.Dokuments.Add(dokumentDB);
                    }
                }
            }
          
            //Zadane Prilohy skopirujeme do newZakazkaDB.Dokuments;
            documentExist = zakazkaZO?.Prilohy?.Any(p => !string.IsNullOrEmpty(p.NazovSuboru)) ?? false;
            if (documentExist)//je zadany aspon jeden subor z Priloh
            {
                var prilohyZO = zakazkaZO?.Prilohy?.Where(p => !string.IsNullOrEmpty(p.NazovSuboru)).ToList();
                if (prilohyZO != null)
                {
                    foreach (BaseItem prilohaZO in prilohyZO)
                    {
                        Dokument dokumentDB = new Dokument();
                        dokumentDB.ZakazkaTg = zakazkaZO?.ZakazkaTg ?? "0";
                        dokumentDB.NazovDokumentu = prilohaZO.NazovDokumentu;
                        dokumentDB.NazovSuboru = prilohaZO.NazovSuboru;
                        dokumentDB.Skupina = prilohaZO.Skupina;
                        dokumentDB.Poznamka = prilohaZO.Poznamka;

                        DokumentDetail detailDB = new DokumentDetail();
                        detailDB.Skupina = prilohaZO.Skupina;

                        if( prilohaZO.FileContent != null) //FileContent pole bytov, obsahuje obrazok z klienta;
                        {
                            detailDB.DokumentContent = new byte[prilohaZO.FileContent.Length];
                            prilohaZO.FileContent?.CopyTo(detailDB.DokumentContent, 0);
                        }
                        dokumentDB.DokumentDetails.Add(detailDB);
                        newZakazkaDB.Dokuments.Add(dokumentDB);
                    }
                }
            }
            return newZakazkaDB;
        }//ConvertZakazkaZO_To_NewZakazka( ref ZakazkaZO zakazkaZO)

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
         * Pri editovani zakazkaZO sa mohli zmenit obsahy  dokumentov, mohol pribudnut dokument (pridal sa subor) do zakazkaZO, mohol sa odobrat dokument (vymazal sa subor) ;
         */
        public void ConvertZakazkaZO_To_Zakazka(ref ZakazkaZO zakazkaZO, ref Zakazka zakazkaDB)  //ked sa robi UPDATE zaznamu z DB!!!!
        {
            if (zakazkaZO == null)
            {
                throw new ArgumentNullException($"Chyba-ConvertZakazkaZO_To_Zakazka: Neplatný údaj pre vstupnú zákazku!");
            }
            if(zakazkaDB == null)
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

            #region == Dokumenty  pre Skupina=1 a Skupina=2 ===

            Dokument? zakazkaTGdokumentDB = zakazkaDB.Dokuments.FirstOrDefault(d => d.Skupina == SkupinaZakazkaTGDokument);
            if (zakazkaTGdokumentDB != null)//zakazkaDB mala uz vytvoreny zakazkaTGdokument, 
            {
                zakazkaTGdokumentDB.Poznamka = zakazkaZO?.ZakazkaTGdokument?.Poznamka?.Trim();
                if(zakazkaZO?.ZakazkaTGdokument?.DokFormFile != null)  //nastala zmena obrazku
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
            if ( existujuPovinneDokumentyZO  )
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
                        pdZO.FileContent.CopyTo(detailDB.DokumentContent,0);
                        
                        dokumentDB.DokumentDetails.Add(detailDB);
                        zakazkaDB.Dokuments.Add(dokumentDB);
                    }
                }
            }
            else if (existujuPovinneDokumentyDB && existujuPovinneDokumentyZO) //zakazkaDB uz ma nejake Povinne dokumenty; zakazkaZO ma vzdy 5 Povinnych Dokumentov, ale nemusia mat zadany NazovSuboru
            {
                for (short i = 0; i < povinneDokumentyZO.Count; i++)
                {
                    bool existujePDZO = zakazkaZO?.PovinneDokumenty?.Any(d => !string.IsNullOrEmpty(d.NazovSuboru) && d.Skupina == SkupinaPrvehoPovinnehoDokumentu + i) ?? false;
                    bool existujePDDB = zakazkaDB.Dokuments.Any(d => !string.IsNullOrEmpty(d.NazovSuboru) && d.Skupina == SkupinaPrvehoPovinnehoDokumentu + i);


                    if (existujePDDB && existujePDZO) //robi sa update-zmena povinneho dokumentu
                    {
                        Dokument? dokumentDB = zakazkaDB.Dokuments.FirstOrDefault(d => !string.IsNullOrEmpty(d.NazovSuboru) && d.Skupina == SkupinaPrvehoPovinnehoDokumentu + i);
                        BaseItem? pdZO = zakazkaZO?.PovinneDokumenty?.FirstOrDefault(d => !string.IsNullOrEmpty(d.NazovSuboru) && d.Skupina == SkupinaPrvehoPovinnehoDokumentu + i);
                        if (dokumentDB != null && pdZO != null) //robi sa update-zmena povinneho dokumentu
                        {
                            //dokumentDB.NazovDokumentu sa nemeni
                            //dokumentDB.Skupina sa nemeni
                            dokumentDB.Poznamka = pdZO?.Poznamka?.Trim();

                            if (pdZO?.DokFormFile != null) //zmenil sa obrazok
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
                    //zakazkaZO ma Povinny dokument kde Skupina=SkupinaPrvehoPovinnehoDokumentu + i
                    //vytvorim novy dokument a pridam ho do zakazkaDB.Dokuments
                    else if (!existujePDDB && existujePDZO)
                    {
                        BaseItem? pdZO = zakazkaZO?.PovinneDokumenty?.FirstOrDefault(d => !string.IsNullOrEmpty(d.NazovSuboru) && d.Skupina == SkupinaPrvehoPovinnehoDokumentu + i);

                        if (pdZO != null)
                        {
                            Dokument dokumentDB = new Dokument();
                            dokumentDB.ZakazkaTg = zakazkaZO.ZakazkaTg;
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
                    else if (existujePDDB && !existujePDZO)
                    {
                        //TODO: treba vymazat z databazy povinny dokument???
                        Dokument? dokumentDB = zakazkaDB.Dokuments.FirstOrDefault(d => !string.IsNullOrEmpty(d.NazovSuboru) && d.Skupina == SkupinaPrvehoPovinnehoDokumentu + i);
                        if (dokumentDB != null)
                            zakazkaDB.Dokuments.Remove(dokumentDB);
                    }
                }
            }//if(!existujuPovinneDokumentyDB....)

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
                //vytvorim nove dokumenty pre zakazkaDB a pridam ich do  zakazkaDB.Dokuments.
                if ( prilohyZO != null )
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

                for (short i = 0; i < zakazkaZO.Prilohy.Count; i++)
                {
                    bool existujePRZO = zakazkaZO?.Prilohy?.Any(d => !string.IsNullOrEmpty(d.NazovSuboru) && d.Skupina == SkupinaPrvejPrilohy + i) ?? false;
                    bool existujePRDB = zakazkaDB?.Dokuments.Any(d => !string.IsNullOrEmpty(d.NazovSuboru) && d.Skupina == SkupinaPrvejPrilohy + i) ?? false;
                    if (existujePRDB && existujePRZO)  //ak sa zmenil NazovSuboru pre obrazok, potom do detailDB vlozim novy obrazok
                    {
                        Dokument? dokumentDB = zakazkaDB?.Dokuments.FirstOrDefault(d => !string.IsNullOrEmpty(d.NazovSuboru) && d.Skupina == SkupinaPrvejPrilohy + i);
                        BaseItem? dokumentZO = zakazkaZO?.Prilohy?.FirstOrDefault(d => !string.IsNullOrEmpty(d.NazovSuboru) && d.Skupina == SkupinaPrvejPrilohy + i);

                        if ((dokumentDB != null)  && (dokumentZO != null))
                        {
                            dokumentDB.NazovDokumentu = dokumentZO.NazovDokumentu;
                            if (dokumentDB.NazovSuboru != dokumentZO.NazovSuboru)//zmenil sa subor pre obrazok
                            {
                                dokumentDB.NazovSuboru = dokumentZO.NazovSuboru; 
                           
                                dokumentDB.Poznamka = dokumentZO?.Poznamka?.Trim();
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
                    //zakazkaZO ma Povinny dokument kde Skupina=SkupinaPrvehoPovinnehoDokumentu + i
                    else if (!existujePRDB &&  existujePRZO) //vytvorim novy dokument a pridam ho do zakazkaDB.Dokuments
                    {
                        BaseItem? dokumentZO = zakazkaZO?.Prilohy?.FirstOrDefault(d => !string.IsNullOrEmpty(d.NazovSuboru) && d.Skupina == SkupinaPrvejPrilohy + i);

                        if (dokumentZO != null)
                        {
                            Dokument dokumentDB = new Dokument();
                            dokumentDB.ZakazkaTg = zakazkaZO.ZakazkaTg.Trim();
                            dokumentDB.NazovDokumentu = dokumentZO.NazovDokumentu;
                            dokumentDB.NazovSuboru = dokumentZO.NazovSuboru;
                            dokumentDB.Skupina = dokumentZO.Skupina;
                            dokumentDB.Poznamka = dokumentZO.Poznamka?.Trim();

                            DokumentDetail dokDetailDB = new DokumentDetail();
                            dokDetailDB.Skupina = dokumentZO.Skupina;
                            if ( dokumentZO.FileContent != null)
                            {
                                dokDetailDB.DokumentContent = new byte[dokumentZO.FileContent.Length];
                                dokumentZO.FileContent.CopyTo(dokDetailDB.DokumentContent, 0);
                            }

                            dokumentDB.DokumentDetails.Add(dokDetailDB);
                            zakazkaDB?.Dokuments.Add(dokumentDB);
                        }
                    }
                    //v zakazkaDB ma vytvorenu Prilohu kde Skupina=SkupinaPrvejPrilohy + i, ale zakazkaZO nema zadany subor pre Prilohu
                    // v zakazkaZO.Prilohy bol vymazany subor kde  Skupina=SkupinaPrvejPrilohy + i
                    else if (existujePRDB && !existujePRZO)
                    {
                        Dokument? dokumentDB = zakazkaDB?.Dokuments.FirstOrDefault(d => !string.IsNullOrEmpty(d.NazovSuboru) && d.Skupina == SkupinaPrvejPrilohy + i);
                        if(dokumentDB != null)
                            zakazkaDB?.Dokuments?.Remove(dokumentDB);
                    }
                }// for (short i = 0; i < zakazkaZO.Prilohy; i++)
            }//if(!existujuPovinneDokumentyDB....)

            #endregion == Prilohy ==

        }//ConvertZakazkaZO_To_Zakazka(ZakazkaZO myZakZO, ref Zakazka zakazkaDB)
    

        /// <summary>
        /// Vytvori instanciu ktora obsahuje vsetky vnorene udaje, ale udaje este nie su nastavene;
        /// Povinne dokumenty obsahuju len NazovDokumentu a cislo skupiny;
        /// Prilohy  obsahuju len NazovDokumentu  a cislo skupiny;
        /// </summary>
        /// <returns></returns>
        public ZakazkaZO VytvorPrazdnuZakazkuZO(short pocetPriloh)
        {
            ZakazkaZO zakazkaZO = new ZakazkaZO();

            VytvorPovinneDokumenty();
            VytvorPrilohy();

            return zakazkaZO;

            int VytvorPovinneDokumenty()//zapise skupinu a NazovDokumentu do kazdej polozky
            {
                for (short i = 0; i < PocetPovinnych; i++)
                {
                    //zakazkaZO.PovinneDokumenty?.Add(new BaseItem() { Skupina = (short)(SkupinaPrvehoPovinnehoDokumentu + i), NazovDokumentu = NazvyPovinnychDokumentov[i], Poznamka = $"Poznamka ABCD{i + 1:000}" });
                    zakazkaZO.PovinneDokumenty?.Add(new BaseItem() { Skupina = (short)(SkupinaPrvehoPovinnehoDokumentu + i), NazovDokumentu = NazvyPovinnychDokumentov[i] });
                }
                return zakazkaZO.PovinneDokumenty?.Count ?? 0;
            }

            int VytvorPrilohy()//zapise skupinu a NazovDokumentu do kazdej polozky
            {
                for (short i = 0; i < pocetPriloh; i++)
                {
                    //zakazkaZO.Prilohy?.Add(new BaseItem() { Skupina = (short)(SkupinaPrvejPrilohy + i), NazovDokumentu = $"Priloha{(SkupinaPrvejPrilohy + i + 1) % SkupinaPrvejPrilohy:00}", Poznamka = $"POZNAMKA Priloha ABCD{(i + 1):000}" });
                    zakazkaZO.Prilohy?.Add(new BaseItem() { Skupina = (short)(SkupinaPrvejPrilohy + i), NazovDokumentu = $"Priloha{(SkupinaPrvejPrilohy + i + 1) % SkupinaPrvejPrilohy:00}", Poznamka = "***" });
                    //zakazkaZO.Prilohy?.Add(new BaseItem() { Skupina = (short)(SkupinaPrvejPrilohy + i), NazovDokumentu = $"Priloha{(SkupinaPrvejPrilohy + i + 1) % SkupinaPrvejPrilohy:00}" });
                }
                return zakazkaZO.Prilohy?.Count ?? 0;
            }
        }
    }
}
