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
    internal partial class ZakazkaServiceWeb : IZakazkaTransformService
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

        //static List<string> NazvyPovinnychDokumentov = new List<string>() { "Pred kalkulacia TG", "Rekapitulacia TG", "Garancna TG", "Registracia TG", "Suhrna faktura" };
        static List<string> NazvyPovinnychDokumentov = new List<string>() { "Garančný protokol", "Predkalkulácia zákazky", "Rekapitulácia zákazky", "Elektronická registrácia práce", "Súhrná faktúra" };

        static short SkupinaZakazkaTGDokument => 1;
        static short SkupinaZakazkaTBDokument => 2;
        static short SkupinaPrvehoPovinnehoDokumentu => 20;
        static short SkupinaPrvejPrilohy => 100;
        static short PocetPovinnych => 5;//Pocet sa nemeni v programe
        static short PocetPriloh => 5;   //Pocet sa moze zvascovat, len na zaciatku sa vytvori 5 priloh;
        static short PocetPriloh1 => 1;  //Pocet sa moze zvascovat, len na zaciatku sa vytvori 1 priloha;

        /// <summary>
        /// Typ instancie Zakazka (z databazy) skonveruje do vytvorenej instancie typu ZakazkaZO (pre zobrazenie pre uzivatela);
        /// Ak sa udaje nacitane z databazy maju len zobrazit uzivatelovi;
        /// </summary>
        /// <param name="zakazkaDB">udaj nacitany z databazy</param>
        /// <returns>ZakazkaZO typ pre editaciu alebo zobrazenie</returns>
        public ZakazkaZO ConvertZakazka_To_ZakazkaZO(ref Zakazka zakazkaDB) //ZOBRAZENIE UDAJOV Z DATABAZY
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
            zakazkaZO.CisloDielu = zakazkaDB?.CisloDielu?.Trim();
            zakazkaZO.CisloFaktury = zakazkaDB?.CisloFaktury?.Trim();
            zakazkaZO.SPZ = zakazkaDB?.Spz?.Trim();
            zakazkaZO.Majitel = zakazkaDB?.Majitel?.Trim();

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
        /// Vytvori prazdnu instanciu typu ZakazkaZO
        /// </summary>
        /// <param name="pocetPriloh"></param>
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
                    zakazkaZO.PovinneDokumenty?.Add(new BaseItem() { Skupina = (short)(SkupinaPrvehoPovinnehoDokumentu + i), NazovDokumentu = NazvyPovinnychDokumentov[i], Poznamka = "*" });
                }
                return zakazkaZO.PovinneDokumenty?.Count ?? 0;
            }

            int VytvorPrilohy()//zapise skupinu a NazovDokumentu do kazdej polozky
            {
                for (short i = 0; i < pocetPriloh; i++)
                {
                    //zakazkaZO.Prilohy?.Add(new BaseItem() { Skupina = (short)(SkupinaPrvejPrilohy + i), NazovDokumentu = $"Priloha{(SkupinaPrvejPrilohy + i + 1) % SkupinaPrvejPrilohy:00}", Poznamka = $"POZNAMKA Priloha ABCD{(i + 1):000}" });
                    zakazkaZO.Prilohy?.Add(new BaseItem() { Skupina = (short)(SkupinaPrvejPrilohy + i), NazovDokumentu = $"Priloha{(SkupinaPrvejPrilohy + i + 1) % SkupinaPrvejPrilohy:00}", Poznamka = "*" });
                    //zakazkaZO.Prilohy?.Add(new BaseItem() { Skupina = (short)(SkupinaPrvejPrilohy + i), NazovDokumentu = $"Priloha{(SkupinaPrvejPrilohy + i + 1) % SkupinaPrvejPrilohy:00}" });
                }
                return zakazkaZO.Prilohy?.Count ?? 0;
            }
        }
    }
}
