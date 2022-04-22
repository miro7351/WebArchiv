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
        static short PocetPriloh => 10;

        /// <summary>
        /// Typ instancie Zakazka (z databazy) skonveruje do vytvorenej instancie typu ZakazkaZO (pre zobrazenie pre uzivatela);
        /// Ak sa udaje nacitane z databazy maju len zobrazit uzivatelovi;
        /// </summary>
        /// <param name="zakazka">udaj nacitany z databazy</param>
        /// <returns>ZakazkaZO typ pre editaciu alebo zobrazenie</returns>
        public ZakazkaZO ConvertZakazka_To_ZakazkaZO(ref Zakazka zakazka)
        {
            ZakazkaZO zakazkaZO = new ZakazkaZO();
            //zakazkaZO.ZakazkaTGdokument neobsahuje obrazok, tj. ZakazkaTGdokument.FileContent=null
            //obrazok sa na poziadanie vyberie z DB!!!

            zakazkaZO.ZakazkaTb = zakazka.ZakazkaTb;
            zakazkaZO.ZakazkaTg = zakazka.ZakazkaTg;
            zakazkaZO.Poznamka = zakazka.Poznamka;
            zakazkaZO.Ukoncena = zakazka.Ukoncena;
            zakazkaZO.Vin = zakazka.Vin;
            zakazkaZO.Cws = zakazka.Cws;
            zakazkaZO.CisloProtokolu = zakazka.CisloProtokolu;
            zakazkaZO.Vytvoril = zakazka.Vytvoril;  
            zakazkaZO.Vytvorene = zakazka.Vytvorene;    
            zakazkaZO.Zmenil = zakazka.Zmenil;
            zakazkaZO.Zmenene = zakazka.Zmenene;    

            var d1 = zakazka.Dokuments.Where(d => d.Skupina == 1).FirstOrDefault();
            //var detaily = d1.DokumentDetails.FirstOrDefault(d => d.Skupina == 1);

            if (zakazka.Dokuments.Any(d => !string.IsNullOrEmpty(d.NazovDokumentu))) //existuju zaznamy pre dokumenty
            {
                //Test ci existuje dokument pre zakazkaZO.ZakazkaTGdokument
                if (zakazka.Dokuments.Any(d => d.Skupina == SkupinaZakazkaTGDokument))//vytvorenie zakazkaZO.ZakazkaTGdokument
                {
                    Dokument? dok = zakazka.Dokuments.FirstOrDefault(d => d.Skupina == SkupinaZakazkaTGDokument);
                    DokumentDetail? detail = dok?.DokumentDetails.FirstOrDefault(d => d.Skupina == SkupinaZakazkaTGDokument);

                    zakazkaZO.ZakazkaTGdokument.NazovDokumentu = dok?.NazovDokumentu;
                    zakazkaZO.ZakazkaTGdokument.NazovSuboru    = dok?.NazovSuboru;

                    zakazkaZO.ZakazkaTGdokument.FileContent = null;

                    zakazkaZO.ZakazkaTGdokument.Skupina   = dok?.Skupina;
                    zakazkaZO.ZakazkaTGdokument.Poznamka  = dok?.Poznamka;
                    zakazkaZO.ZakazkaTGdokument.Vytvoril  = dok?.Vytvoril;
                    zakazkaZO.ZakazkaTGdokument.Vytvorene = dok?.Vytvorene;
                    zakazkaZO.ZakazkaTGdokument.Zmenil    = dok?.Zmenil;
                    zakazkaZO.ZakazkaTGdokument.Zmenene   = dok?.Zmenene;
                }

                //Test ci existuje dokument pre zakazkaZO.ZakazkaTGdokument
                if (zakazka.Dokuments.Any(d => d.Skupina == SkupinaZakazkaTBDokument))//vytvorenie zakazkaZO.ZakazkaTBdokument
                {
                    Dokument? dok = zakazka.Dokuments.FirstOrDefault(d => d.Skupina == SkupinaZakazkaTBDokument);
                   // DokumentDetail detail = dok?.DokumentDetails.FirstOrDefault(d => d.Skupina == SkupinaZakazkaTBDokument);

                    zakazkaZO.ZakazkaTBdokument.NazovDokumentu = dok?.NazovDokumentu;
                    zakazkaZO.ZakazkaTBdokument.NazovSuboru = dok?.NazovSuboru;

                    zakazkaZO.ZakazkaTGdokument.FileContent = null;
                   
                    zakazkaZO.ZakazkaTBdokument.Skupina = dok?.Skupina;
                    zakazkaZO.ZakazkaTBdokument.Poznamka = dok?.Poznamka;
                    zakazkaZO.ZakazkaTBdokument.Vytvoril = dok?.Vytvoril;
                    zakazkaZO.ZakazkaTBdokument.Vytvorene = dok?.Vytvorene;
                    zakazkaZO.ZakazkaTBdokument.Zmenil = dok?.Zmenil;
                    zakazkaZO.ZakazkaTBdokument.Zmenene = dok?.Zmenene;
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
                                bi.Skupina = dok.Skupina;
                                bi.NazovDokumentu = dok.NazovDokumentu;  //
                                bi.Poznamka = dok.Poznamka;
                                bi.NazovSuboru = dok.NazovSuboru;
                                bi.Vytvoril = dok.Vytvoril;
                                bi.Vytvorene = dok.Vytvorene;
                                bi.Zmenil = dok.Zmenil;
                                bi.Zmenene = dok.Zmenene;
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
                            bi.NazovDokumentu = dok1.NazovDokumentu;  //
                            
                            bi.Poznamka = dok1.Poznamka;
                            bi.NazovSuboru = dok1.NazovSuboru;
                            bi.Vytvoril = dok1.Vytvoril;
                            bi.Vytvorene = dok1.Vytvorene;
                            bi.Zmenil = dok1.Zmenil;
                            bi.Zmenene = dok1.Zmenene;
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
                #endregion == Prilohy ==
            }//if( zakazka.Dokuments.Any(d => !string.IsNullOrEmpty(d.NazovDokumentu)) ) //existuju zaznamy pre dokumety
            return zakazkaZO;
        }//ConvertZakazka_To_ZakazkaZO(Zakazka zakazka)


        /// <summary>
        /// Typ instancie ZakazkaZO skonveruje do NOVEJ instancie Typu Zakazka;
        /// Ak sa udaje  zadane uzivatelom maju INSERTOVAT do databazy;
        /// </summary>
        /// <param name="myZakZO">Udaj editovany uzivatelom</param>
        /// <returns>Udaj ktory sa ma zapisat do databazy</returns>
        public  Zakazka ConvertZakazkaZO_To_Zakazka(ref ZakazkaZO myZakZO)  //Ak sa vytvara nova instancia typu Zakazka pre INSERT do DB!!!
        {
            Zakazka newZakazka = new Zakazka();

            newZakazka.ZakazkaTb = myZakZO.ZakazkaTb;
            newZakazka.ZakazkaTg = myZakZO.ZakazkaTg;
            newZakazka.Poznamka = myZakZO.Poznamka;
            newZakazka.Ukoncena = myZakZO.Ukoncena;
            newZakazka.Vin = myZakZO.Vin;
            newZakazka.Cws = myZakZO.Cws;
            newZakazka.CisloProtokolu = myZakZO.CisloProtokolu;

            //12.04.2022 17:15
            //NB Dell
            //Cannot insert the value NULL into column 'Skupina', table 'Toyota_DB2.dbo.Dokument'; column does not allow nulls.INSERT fails.
            //The statement has been terminated.
            //tab Dokument ma Poznamka nvarchar(128) not null

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
                        dokument1.ZakazkaTg = myZakZO?.ZakazkaTg ?? "0";
                        dokument1.NazovDokumentu = povinnyDokument.NazovDokumentu;
                        dokument1.NazovSuboru = povinnyDokument.NazovSuboru;
                        dokument1.Skupina = povinnyDokument.Skupina;
                        dokument1.Poznamka = povinnyDokument.Poznamka;

                        DokumentDetail dokDetail1 = new DokumentDetail();
                        
                        dokDetail1.Skupina = povinnyDokument.Skupina;
                        if( povinnyDokument.FileContent != null)
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
                var prilohy = myZakZO?.Prilohy?.Where(p => !string.IsNullOrEmpty(p.FilePath)).ToList();
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

                        if( priloha.FileContent != null)
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

        /*
         * instancia myZakZO vzikla z instancie zakazkaDB, pri editovani myZakZO sa mohli zmenit obsahy instancii dokumentov, mohol pribudnut dokument do myZakZO;
         */
        public  void ConvertZakazkaZO_To_Zakazka(ref ZakazkaZO myZakZO, ref Zakazka zakazkaDB)  //ked sa robi UPDATE zaznamu z DB!!!!
        {

            //zakazkaDB.ZakazkaTg = myZakZO.ZakazkaTg;   //ZakazkaTg sa nemeni!!!
            zakazkaDB.ZakazkaTb = myZakZO.ZakazkaTb;

            //zakazkaDB.Platna    = myZakZO.Platna; 
            zakazkaDB.Poznamka = myZakZO.Poznamka;
            zakazkaDB.Ukoncena = myZakZO.Ukoncena;
            zakazkaDB.Vin = myZakZO.Vin;
            zakazkaDB.Cws = myZakZO.Cws;
            zakazkaDB.CisloProtokolu = myZakZO.CisloProtokolu;

            #region == Dokumenty  pre Skupina=1 a Skupina=2 ===

            var zakazkaTGdokument = zakazkaDB.Dokuments.FirstOrDefault(d => d.Skupina == SkupinaZakazkaTGDokument);
            if (zakazkaTGdokument != null)//zakazkaDB mala uz zakazkaTGdokument
            {
                zakazkaTGdokument.Poznamka = myZakZO.ZakazkaTGdokument.Poznamka;
                //zakazkaTBdokument.NazovDokumentu = myZakZO.ZakazkaTBdokument.NazovDokumentu;  nemeni sa
                zakazkaTGdokument.NazovSuboru = myZakZO.ZakazkaTGdokument.NazovSuboru;
                //zakazkaTBdokument.Skupina = myZakZO.ZakazkaTBdokument.Skupina;   nemeni sa
            }
            else//zakazkaDB este nemala vytvoreny Dokument pre Skupina=1
            {
                Dokument dokument1 = new Dokument();
                dokument1.ZakazkaTg = myZakZO.ZakazkaTg;
                dokument1.NazovDokumentu = myZakZO.ZakazkaTGdokument.NazovDokumentu;
                dokument1.NazovSuboru = myZakZO.ZakazkaTGdokument.NazovSuboru;
                dokument1.Skupina = SkupinaZakazkaTGDokument;
                //dokument1.Poznamka = myZakZO.ZakazkaTBdokument.Poznamka  sa nenastavuje

                DokumentDetail dokDetail1 = new DokumentDetail();
                dokDetail1.Skupina = SkupinaZakazkaTGDokument;
                dokDetail1.DokumentContent = System.IO.File.ReadAllBytes(myZakZO.ZakazkaTGdokument.FilePath);

                dokument1.DokumentDetails.Add(dokDetail1);
                zakazkaDB.Dokuments.Add(dokument1);
            }

            var zakazkaTBdokument = zakazkaDB.Dokuments.FirstOrDefault(d => d.Skupina == SkupinaZakazkaTBDokument);
            if (zakazkaTBdokument != null)//zakazkaDB mala uz zakazkaTBdokument
            {
                zakazkaTBdokument.Poznamka = myZakZO.ZakazkaTBdokument.Poznamka;
                //zakazkaTBdokument.NazovDokumentu = myZakZO.ZakazkaTBdokument.NazovDokumentu;  nemeni sa
                zakazkaTBdokument.NazovSuboru = myZakZO.ZakazkaTBdokument.NazovSuboru;
                //zakazkaTBdokument.Skupina = myZakZO.ZakazkaTBdokument.Skupina;   nemenisa
            }
            else//zakazkaDB este nemala vytvoreny Dokument pre Skupina=2
            {
                Dokument dokument1 = new Dokument();
                dokument1.ZakazkaTg = myZakZO.ZakazkaTg;
                dokument1.NazovDokumentu = myZakZO.ZakazkaTBdokument.NazovDokumentu;
                dokument1.NazovSuboru = myZakZO.ZakazkaTBdokument.NazovSuboru;
                dokument1.Skupina = SkupinaZakazkaTBDokument;
                //dokument1.Poznamka = myZakZO.ZakazkaTBdokument.Poznamka  sa nenastavuje

                DokumentDetail dokDetail1 = new DokumentDetail();
                dokDetail1.Skupina = SkupinaZakazkaTBDokument;
                dokDetail1.DokumentContent = System.IO.File.ReadAllBytes(myZakZO.ZakazkaTBdokument.FilePath);

                dokument1.DokumentDetails.Add(dokDetail1);
                zakazkaDB.Dokuments.Add(dokument1);
            }

            #endregion == Dokumenty  pre Skupina=1 a Skupina=2 ===

            #region == Povinne dokumenty ==

            List<BaseItem>? povinneDokumentyZO = null;
            List<Dokument>? povinneDokumentyDB = null;

            bool? existujuPovinneDokumentyZO = myZakZO?.PovinneDokumenty?.Any(d => !string.IsNullOrEmpty(d.NazovSuboru));
            if (existujuPovinneDokumentyZO.HasValue && existujuPovinneDokumentyZO.Value)
                povinneDokumentyZO = myZakZO?.PovinneDokumenty?.Where(d => !string.IsNullOrEmpty(d.NazovSuboru)).ToList();

            bool existujuPovinneDokumentyDB = zakazkaDB.Dokuments.Any(d => d.Skupina >= SkupinaPrvehoPovinnehoDokumentu && d.Skupina < SkupinaPrvejPrilohy);

            if (existujuPovinneDokumentyDB)
                povinneDokumentyDB = zakazkaDB.Dokuments.Where(d => d.Skupina >= SkupinaPrvehoPovinnehoDokumentu && d.Skupina < SkupinaPrvejPrilohy).ToList();

            if (!existujuPovinneDokumentyDB && existujuPovinneDokumentyZO.HasValue && existujuPovinneDokumentyZO.Value)//zakazkaDB este nemala vytvorene Povinne dokumenty
            {
                //vytvorim nove dokumenty a vlozim ich do  zakazkaDB.Dokuments.
                if (povinneDokumentyZO != null)
                {
                    foreach (var dokument in povinneDokumentyZO)
                    {
                        Dokument dokument1 = new Dokument();
                        dokument1.ZakazkaTg = myZakZO.ZakazkaTg;
                        dokument1.NazovDokumentu = dokument.NazovDokumentu;
                        dokument1.NazovSuboru = dokument.NazovSuboru;
                        dokument1.Skupina = dokument.Skupina;
                        dokument1.Poznamka = dokument.Poznamka;

                        DokumentDetail dokDetail1 = new DokumentDetail();
                        dokDetail1.Skupina = dokument.Skupina;
                        //dokDetail1.DokumentContent = System.IO.File.ReadAllBytes(dokument.FilePath);

                        dokument1.DokumentDetails.Add(dokDetail1);
                        zakazkaDB.Dokuments.Add(dokument1);
                    }
                }
            }
            else if (existujuPovinneDokumentyDB && existujuPovinneDokumentyZO.HasValue && existujuPovinneDokumentyZO.Value) //zakazkaDB uz ma nejake Povinne dokumenty; myZakZO ma vzdy 5 Povinnych Dokumentov, ale nemusia mat zadany NazovSuboru
            {
                for (short i = 0; i < PocetPovinnych; i++)
                {
                    bool? existujePDZO = myZakZO?.PovinneDokumenty?.Any(d => !string.IsNullOrEmpty(d.NazovSuboru) && d.Skupina == SkupinaPrvehoPovinnehoDokumentu + i);
                    bool existujePDDB = zakazkaDB.Dokuments.Any(d => !string.IsNullOrEmpty(d.NazovSuboru) && d.Skupina == SkupinaPrvehoPovinnehoDokumentu + i);


                    if (existujePDDB && existujePDZO.HasValue && existujePDZO.Value)
                    {
                        Dokument? dokumentDB = zakazkaDB.Dokuments.FirstOrDefault(d => !string.IsNullOrEmpty(d.NazovSuboru) && d.Skupina == SkupinaPrvehoPovinnehoDokumentu + i);
                        BaseItem? dokumentZO = myZakZO?.PovinneDokumenty?.FirstOrDefault(d => !string.IsNullOrEmpty(d.NazovSuboru) && d.Skupina == SkupinaPrvehoPovinnehoDokumentu + i);
                        if (dokumentDB != null)
                        {
                            dokumentDB.NazovSuboru = dokumentZO?.NazovSuboru;
                            dokumentDB.Poznamka = dokumentZO?.Poznamka;
                            //TODO: nastavit Dokument Deatil!!!!
                        }
                    }
                    //v zakazkaDB este nema vytvoreny Povinny dokument kde Skupina=SkupinaPrvehoPovinnehoDokumentu + i
                    //myZakZo ma Povinny dokument kde Skupina=SkupinaPrvehoPovinnehoDokumentu + i
                    //vytvorim novy dokument a pridam ho do zakazkaDB.Dokuments
                    else if (!existujePDDB && existujePDZO.HasValue && existujePDZO.Value)
                    {
                        BaseItem? dokumentZO = myZakZO?.PovinneDokumenty?.FirstOrDefault(d => !string.IsNullOrEmpty(d.NazovSuboru) && d.Skupina == SkupinaPrvehoPovinnehoDokumentu + i);

                        if (dokumentZO != null)
                        {
                            Dokument dokument1 = new Dokument();
                            dokument1.ZakazkaTg = myZakZO.ZakazkaTg;
                            dokument1.NazovDokumentu = dokumentZO.NazovDokumentu;
                            dokument1.NazovSuboru = dokumentZO.NazovSuboru;
                            dokument1.Skupina = dokumentZO.Skupina;
                            dokument1.Poznamka = dokumentZO.Poznamka;

                            DokumentDetail dokDetail1 = new DokumentDetail();
                            dokDetail1.Skupina = dokumentZO.Skupina;
                            //dokDetail1.DokumentContent = System.IO.File.ReadAllBytes(dokumentZO.FilePath);

                            dokument1.DokumentDetails.Add(dokDetail1);
                            zakazkaDB.Dokuments.Add(dokument1);
                        }
                    }
                    //v zakazkaDB ma vytvoreny Povinny dokument kde Skupina=SkupinaPrvehoPovinnehoDokumentu + i, ale myZakZO nema zadany subor pre Povinny dokument
                    // v myZakZo.PovinneDokumenty bol vymazany subor kde  Skupina=SkupinaPrvehoPovinnehoDokumentu + i
                    else if (existujePDDB && existujePDZO.HasValue && !existujePDZO.Value)
                    {
                        Dokument? dokumentDB = zakazkaDB.Dokuments.FirstOrDefault(d => !string.IsNullOrEmpty(d.NazovSuboru) && d.Skupina == SkupinaPrvehoPovinnehoDokumentu + i);
                        if (dokumentDB != null)
                        {
                            zakazkaDB.Dokuments.Remove(dokumentDB);
                        }
                    }
                }
            }//if(!existujuPovinneDokumentyDB....)

            #endregion == Povinne dokumenty ==

            #region == Prilohy ==

            List<BaseItem>? prilohyZO = null;
            List<Dokument>? prilohyDB = null;

            bool? existujuPrilohyZO = myZakZO?.Prilohy?.Any(d => !string.IsNullOrEmpty(d.NazovSuboru));

            if (existujuPrilohyZO.HasValue && existujuPrilohyZO.Value)
                prilohyZO = myZakZO?.Prilohy?.Where(d => !string.IsNullOrEmpty(d.NazovSuboru)).ToList();

            bool existujuPrilohyDB = zakazkaDB.Dokuments.Any(d => d.Skupina >= SkupinaPrvejPrilohy);

            if (existujuPrilohyDB)
                prilohyDB = zakazkaDB.Dokuments.Where(d => d.Skupina >= SkupinaPrvejPrilohy).ToList();

            if (!existujuPrilohyDB && existujuPrilohyZO.HasValue && existujuPrilohyZO.Value)//zakazkaDB este nemala vytvorene Prilohy
            {
                //vytvorim nove dokumenty a vlozim ich do  zakazkaDB.Dokuments.
                if( (prilohyDB != null) && (prilohyZO != null) )
                {
                    foreach (var dokument in prilohyZO)
                    {
                        Dokument dokument1 = new Dokument();
                        dokument1.ZakazkaTg = myZakZO.ZakazkaTg;
                        dokument1.NazovDokumentu = dokument.NazovDokumentu;
                        dokument1.NazovSuboru = dokument.NazovSuboru;
                        dokument1.Skupina = dokument.Skupina;
                        dokument1.Poznamka = dokument.Poznamka;

                        DokumentDetail dokDetail1 = new DokumentDetail();
                        dokDetail1.Skupina = dokument.Skupina;
                        //dokDetail1.DokumentContent = System.IO.File.ReadAllBytes(dokument.FilePath);
                        //TODO: dokoncit!!!
                        dokument1.DokumentDetails.Add(dokDetail1);
                        zakazkaDB.Dokuments.Add(dokument1);
                    }
                }
            }
            else if (existujuPrilohyDB && existujuPrilohyZO.HasValue && existujuPrilohyZO.Value)//zakazkaDB uz ma nejake Prilohy;
            {

                for (short i = 0; i < PocetPriloh; i++)
                {
                    bool? existujePRZO = myZakZO?.Prilohy?.Any(d => !string.IsNullOrEmpty(d.NazovSuboru) && d.Skupina == SkupinaPrvejPrilohy + i);
                    bool existujePRDB = zakazkaDB.Dokuments.Any(d => !string.IsNullOrEmpty(d.NazovSuboru) && d.Skupina == SkupinaPrvejPrilohy + i);
                    if (existujePRDB && existujePRZO.HasValue && existujePRZO.Value)
                    {
                        Dokument? dokumentDB = zakazkaDB.Dokuments.FirstOrDefault(d => !string.IsNullOrEmpty(d.NazovSuboru) && d.Skupina == SkupinaPrvejPrilohy + i);
                        BaseItem? dokumentZO = myZakZO?.Prilohy?.FirstOrDefault(d => !string.IsNullOrEmpty(d.NazovSuboru) && d.Skupina == SkupinaPrvejPrilohy + i);

                        if ((dokumentDB != null)  && (dokumentZO != null))
                        {
                            if (dokumentDB.NazovSuboru != dokumentZO.NazovSuboru)
                            {
                                dokumentDB.NazovSuboru = dokumentZO.NazovSuboru; //TODO: este urobit DokumentDetail!!!! aj dalej v kode!!!!
                            }
                            dokumentDB.Poznamka = dokumentZO.Poznamka;
                        }
                        
                    }
                    //v zakazkaDB este nema vytvoreny Povinny dokument kde Skupina=SkupinaPrvehoPovinnehoDokumentu + i
                    //myZakZo ma Povinny dokument kde Skupina=SkupinaPrvehoPovinnehoDokumentu + i
                    //vytvorim novy dokument a pridam ho do zakazkaDB.Dokuments
                    else if (!existujePRDB &&  existujePRZO.HasValue && existujePRZO.Value)
                    {
                        BaseItem? dokumentZO = myZakZO?.Prilohy?.FirstOrDefault(d => !string.IsNullOrEmpty(d.NazovSuboru) && d.Skupina == SkupinaPrvejPrilohy + i);

                        if (dokumentZO != null)
                        {
                            Dokument dokument1 = new Dokument();
                            dokument1.ZakazkaTg = myZakZO.ZakazkaTg;
                            dokument1.NazovDokumentu = dokumentZO.NazovDokumentu;
                            dokument1.NazovSuboru = dokumentZO.NazovSuboru;
                            dokument1.Skupina = dokumentZO.Skupina;
                            dokument1.Poznamka = dokumentZO.Poznamka;

                            DokumentDetail dokDetail1 = new DokumentDetail();
                            dokDetail1.Skupina = dokumentZO.Skupina;
                            //dokDetail1.DokumentContent = System.IO.File.ReadAllBytes(dokumentZO.FilePath);

                            dokument1.DokumentDetails.Add(dokDetail1);
                            zakazkaDB.Dokuments.Add(dokument1);
                        }
                    }
                    //v zakazkaDB ma vytvorenu Prilohu kde Skupina=SkupinaPrvejPrilohy + i, ale myZakZO nema zadany subor pre Prilohu
                    // v myZakZO.Prilohy bol vymazany subor kde  Skupina=SkupinaPrvejPrilohy + i
                    else if (existujePRDB && existujePRZO.HasValue && !existujePRZO.Value)
                    {
                        Dokument? dokumentDB = zakazkaDB.Dokuments.FirstOrDefault(d => !string.IsNullOrEmpty(d.NazovSuboru) && d.Skupina == SkupinaPrvejPrilohy + i);
                        if(dokumentDB != null)
                            zakazkaDB.Dokuments.Remove(dokumentDB);
                        // zakazkaDB.Dokuments.Remove(dokumentDB!); skrateny zapis
                    }
                }// for (short i = 0; i < PocetPriloh; i++)
            }//if(!existujuPovinneDokumentyDB....)

            #endregion == Prilohy ==

        }//ConvertZakazkaZO_To_Zakazka(ZakazkaZO myZakZO, ref Zakazka zakazkaDB)
    }
}
