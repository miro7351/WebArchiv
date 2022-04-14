using PA.TOYOTA.DB;
using ToyotaArchiv.Domain;
using ToyotaArchiv.Interfaces;

namespace ToyotaArchiv.Services
{
    public class ZakazkaTransformService : IZakazkaTransformService
    { 
        public ZakazkaTransformService()
        {

        }

        /* Nazvy pre Povinne dokumenty
         Name = "Pred kalkulacia TG"
         Name = "Rekapitulacia TG", 
         Name = "Garancna TG", 
         Name = "Registracia TG",
         Name = "Suhrna faktura"

        */

         List<string> NazvyPovinnychDokumentov = new List<string>() { "Pred kalkulacia TG", "Rekapitulacia TG", "Garancna TG", "Registracia TG", "Suhrna faktura" };
        int SkupinaPrvehoPovinnehoDokumentu => 20;
        int SkupinaPrvejPrilohy => 100;
        int PocetPovinnych => 5;
        int PocetPriloh => 10;

        /// <summary>
        /// Typ instancie Zakazka (z databazy)skonveruje do instancie typu ZakazkaZO (pre zobrazenie pre uzivatela);
        /// Ak sa udaje nacitane z databazy maju zobrazit uzivatelovi;
        /// </summary>
        /// <param name="zakazka">udaj nacitany z databazy</param>
        /// <returns>ZakazkaZO typ pre editaciu alebo zobrazenie</returns>
        public ZakazkaZO ConvertZakazka_To_ZakazkaZO(Zakazka zakazka)
        {
            ZakazkaZO zakazkaZO = new ZakazkaZO();
            zakazkaZO.ZakazkaTb = zakazka.ZakazkaTb;
            zakazkaZO.ZakazkaTg = zakazka.ZakazkaTg;
            zakazkaZO.Platna = zakazka.Platna;
            zakazkaZO.Poznamka = zakazka.Poznamka;
            zakazkaZO.Ukoncena = zakazka.Ukoncena;
            zakazkaZO.Vin = zakazka.Vin;
            zakazkaZO.Cws = zakazka.Cws;
            zakazkaZO.CisloProtokolu = zakazka.CisloProtokolu;

            if (zakazka.Dokuments.Any(d => !string.IsNullOrEmpty(d.NazovDokumentu))) //existuju zaznamy pre dokumety
            {
                if (zakazka.Dokuments.Any(d => d.Skupina == 1))//zakazkaZO.ZakazkaTGdokument
                {
                    Dokument dok = zakazka.Dokuments.FirstOrDefault(d => d.Skupina == 1);
                    zakazkaZO.ZakazkaTGdokument.NazovDokumentu = dok.NazovDokumentu;
                    zakazkaZO.ZakazkaTGdokument.NazovSuboru = dok.NazovSuboru;
                    zakazkaZO.ZakazkaTGdokument.DokumentPlatny = dok.DokumentPlatny;
                    zakazkaZO.ZakazkaTGdokument.Skupina = dok.Skupina;
                    zakazkaZO.ZakazkaTGdokument.Poznamka = dok.Poznamka;
                    zakazkaZO.ZakazkaTGdokument.Vytvoril = dok.Vytvoril;
                    zakazkaZO.ZakazkaTGdokument.Vytvorene = dok.Vytvorene;
                    zakazkaZO.ZakazkaTGdokument.Zmenil = dok.Zmenil;
                    zakazkaZO.ZakazkaTGdokument.Zmenene = dok.Zmenene;
                }
                if (zakazka.Dokuments.Any(d => d.Skupina == 2))//zakazkaZO.ZakazkaTBdokument
                {
                    Dokument dok = zakazka.Dokuments.FirstOrDefault(d => d.Skupina == 2);
                    zakazkaZO.ZakazkaTBdokument.NazovDokumentu = dok.NazovDokumentu;
                    zakazkaZO.ZakazkaTBdokument.NazovSuboru = dok.NazovSuboru;
                    zakazkaZO.ZakazkaTBdokument.DokumentPlatny = dok.DokumentPlatny;
                    zakazkaZO.ZakazkaTBdokument.Skupina = dok.Skupina;
                    zakazkaZO.ZakazkaTBdokument.Poznamka = dok.Poznamka;
                    zakazkaZO.ZakazkaTBdokument.Vytvoril = dok.Vytvoril;
                    zakazkaZO.ZakazkaTBdokument.Vytvorene = dok.Vytvorene;
                    zakazkaZO.ZakazkaTBdokument.Zmenil = dok.Zmenil;
                    zakazkaZO.ZakazkaTBdokument.Zmenene = dok.Zmenene;
                }

                if (zakazka.Dokuments.Any(d => d.Skupina >= SkupinaPrvehoPovinnehoDokumentu && d.Skupina < SkupinaPrvejPrilohy))//Povinne dokumenty
                {
                    //pocet zaznamov pre povinne dokumety
                    int pz = zakazka.Dokuments.Count(d => d.Skupina >= SkupinaPrvehoPovinnehoDokumentu && d.Skupina < SkupinaPrvejPrilohy);

                    for (int i = 0; i < PocetPovinnych; i++)
                    {
                        Dokument dok = zakazka.Dokuments.FirstOrDefault(d => d.Skupina == (SkupinaPrvehoPovinnehoDokumentu + i));
                        BaseItem bi = new BaseItem();
                        if (dok != null) //existuje zaznam kde Skupina= (SkupinaPrvehoPovinnehoDokumentu+i)
                        {
                            bi.Skupina = dok.Skupina;
                            bi.NazovDokumentu = dok.NazovDokumentu;  //
                            bi.DokumentPlatny = dok.DokumentPlatny;
                            bi.Poznamka = dok.Poznamka;
                            bi.NazovSuboru = dok.NazovSuboru;
                            bi.Vytvoril = dok.Vytvoril;
                            bi.Vytvorene = dok.Vytvorene;
                            bi.Zmenil = dok.Zmenil;
                            bi.Zmenene = dok.Zmenene;
                        }
                        else //neexistuje zaznam, nastavim  len jeho property NazovDokumentu a Skupina
                        {
                            bi.Skupina = SkupinaPrvehoPovinnehoDokumentu + i;

                            if (i < NazvyPovinnychDokumentov.Count)
                            {
                                bi.NazovDokumentu = NazvyPovinnychDokumentov[i];
                            }
                            else
                            {
                                //vytvorim Nazov dokumentu
                                bi.NazovDokumentu = $"Povinny dok{SkupinaPrvehoPovinnehoDokumentu + i}";
                                //vytvorim jeho skupinu
                                bi.Skupina = SkupinaPrvehoPovinnehoDokumentu + i;
                            }
                        }
                        zakazkaZO.PovinneDokumenty.Add(bi);
                    }//for
                }

                if (zakazka.Dokuments.Any(d => d.Skupina >= SkupinaPrvejPrilohy))//Prilohy
                {
                    //pocet zaznamov pre prilohy
                    int pz = zakazka.Dokuments.Count(d => d.Skupina >= SkupinaPrvejPrilohy);

                    if (pz > 0)
                    {
                        var dokumentyPrilohy = zakazka.Dokuments.Where(d => d.Skupina >= SkupinaPrvejPrilohy).ToList();
                        foreach (var dok1 in dokumentyPrilohy)
                        {
                            BaseItem bi = new BaseItem();
                            bi.Skupina = dok1.Skupina;
                            bi.NazovDokumentu = dok1.NazovDokumentu;  //
                            bi.DokumentPlatny = dok1.DokumentPlatny;
                            bi.Poznamka = dok1.Poznamka;
                            bi.NazovSuboru = dok1.NazovSuboru;
                            bi.Vytvoril = dok1.Vytvoril;
                            bi.Vytvorene = dok1.Vytvorene;
                            bi.Zmenil = dok1.Zmenil;
                            bi.Zmenene = dok1.Zmenene;
                            zakazkaZO.Prilohy.Add(bi);
                        }//for
                    }
                    //doplnenie do poctu 10 priloh, nastavim Skupinu a NazovDokumentu PrilohaXX
                    if (PocetPriloh > pz)
                    {
                        for (int i = 0; i < PocetPriloh - pz; i++)
                        {
                            BaseItem bi = new BaseItem();
                            bi.Skupina = SkupinaPrvejPrilohy + pz + i;
                            bi.NazovDokumentu = $"Priloha{(SkupinaPrvejPrilohy + pz + i) % SkupinaPrvejPrilohy:00}";
                            zakazkaZO.Prilohy.Add(bi);
                        }
                    }
                }
            }//if( zakazka.Dokuments.Any(d => !string.IsNullOrEmpty(d.NazovDokumentu)) ) //existuju zaznamy pre dokumety
            return zakazkaZO;
        }


        /// <summary>
        /// Typ instancie ZakazkaZO skonveruje do instancie Typu Zakazka;
        /// Ak sa udaje  zadane uzivatelom maju zapisat do databazy;
        /// </summary>
        /// <param name="myZakZO">Udaj editovany uzivatelom</param>
        /// <returns>Udaj ktory sa ma zapisat do databazy</returns>
        public Zakazka ConvertZakazkaZO_To_Zakazka(ZakazkaZO myZakZO)
        {
            Zakazka newZakazka = new Zakazka();

            newZakazka.ZakazkaTb = myZakZO.ZakazkaTb;
            newZakazka.ZakazkaTg = myZakZO.ZakazkaTg;
            newZakazka.Platna = myZakZO.Platna;
            newZakazka.Poznamka = myZakZO.Poznamka;
            newZakazka.Ukoncena = myZakZO.Ukoncena;
            newZakazka.Vin = myZakZO.Vin;
            newZakazka.Cws = myZakZO.Cws;
            newZakazka.CisloProtokolu = myZakZO.CisloProtokolu;

           
            if (!string.IsNullOrEmpty(myZakZO.ZakazkaTGdokument.FilePath))//je zadany subor; vytvorit instanciu typu Dokument a instanciu typu Document_Detail;
            {
                Dokument dokument1 = new Dokument();
                dokument1.ZakazkaTg = newZakazka.ZakazkaTg;
                dokument1.NazovDokumentu = myZakZO.ZakazkaTGdokument.NazovDokumentu;
                dokument1.NazovSuboru = myZakZO.ZakazkaTGdokument.NazovSuboru;
                dokument1.Skupina = (short)1;
                dokument1.DokumentPlatny = myZakZO.ZakazkaTGdokument.DokumentPlatny;
                dokument1.Poznamka = "*";

                DokumentDetail dokDetail1 = new DokumentDetail();
                dokDetail1.NazovDokumentu = myZakZO.ZakazkaTGdokument.NazovDokumentu;
                dokDetail1.Platny = myZakZO.ZakazkaTGdokument.DokumentPlatny;
                dokDetail1.Poznamka = string.Empty;// myZakZO.ZakazkaTGdokument.Poznamka;
                //TODO: ???
                //dokDetail1.DocumentContent = System.IO.File.ReadAllBytes(myZakZO.ZakazkaTGdokument.FilePath);

                dokument1.DokumentDetails.Add(dokDetail1);
                newZakazka.Dokuments.Add(dokument1);
            }

            if (!string.IsNullOrEmpty(myZakZO.ZakazkaTBdokument.FilePath))//je zadany subor; vytvorit instanciu typu Dokument a instanciu typu Document_Detail;
            {
                Dokument dokument1 = new Dokument();
                dokument1.ZakazkaTg = newZakazka.ZakazkaTg;
                dokument1.NazovDokumentu = myZakZO.ZakazkaTBdokument.NazovDokumentu;
                dokument1.NazovSuboru = myZakZO.ZakazkaTBdokument.NazovSuboru;
                dokument1.Skupina = (short)2;
                dokument1.DokumentPlatny = myZakZO.ZakazkaTBdokument.DokumentPlatny;
                dokument1.Poznamka = "*"; // string.Empty;



                DokumentDetail dokDetail1 = new DokumentDetail();
                dokDetail1.NazovDokumentu = myZakZO.ZakazkaTBdokument.NazovDokumentu;
                dokDetail1.Platny = myZakZO.ZakazkaTBdokument.DokumentPlatny;
                dokDetail1.Poznamka = string.Empty;
                //TODO: ???
                //dokDetail1.DocumentContent = System.IO.File.ReadAllBytes(myZakZO.ZakazkaTBdokument.FilePath);

                dokument1.DokumentDetails.Add(dokDetail1);
                newZakazka.Dokuments.Add(dokument1);
            }

            var status = myZakZO.PovinneDokumenty.Any(p => !string.IsNullOrEmpty(p.FilePath));
            if (status)//je zadany aspon jeden subor
            {
                var povinneDokumenty = myZakZO.PovinneDokumenty.Where(p => !string.IsNullOrEmpty(p.FilePath)).ToList();
                foreach (var dokument in povinneDokumenty)
                {
                    Dokument dokument1 = new Dokument();
                    dokument1.ZakazkaTg = newZakazka.ZakazkaTg;
                    dokument1.NazovDokumentu = dokument.NazovDokumentu;
                    dokument1.NazovSuboru = dokument.NazovSuboru;
                    dokument1.Skupina = (short)dokument.Skupina;
                    dokument1.DokumentPlatny = dokument.DokumentPlatny ?? "A";
                    dokument1.Poznamka = dokument.Poznamka ?? " ";

                    DokumentDetail dokDetail1 = new DokumentDetail();
                    dokDetail1.NazovDokumentu = dokument.NazovDokumentu;
                    dokDetail1.Platny = dokument.DokumentPlatny ?? "A";
                    dokDetail1.Poznamka = string.Empty;
                    //TODO: ???
                    //dokDetail1.DocumentContent = System.IO.File.ReadAllBytes(myZakZO.ZakazkaTBdokument.FilePath);

                    dokument1.DokumentDetails.Add(dokDetail1);
                    newZakazka.Dokuments.Add(dokument1);
                }
            }

            status = myZakZO.Prilohy.Any(p => !string.IsNullOrEmpty(p.FilePath));
            if (status)//je zadany aspon jeden subor
            {
                var prilohy = myZakZO.Prilohy.Where(p => !string.IsNullOrEmpty(p.FilePath)).ToList();
                foreach (var dokument in prilohy)
                {
                    Dokument dokument1 = new Dokument();
                    dokument1.ZakazkaTg = newZakazka.ZakazkaTg;
                    dokument1.NazovDokumentu = dokument.NazovDokumentu;
                    dokument1.NazovSuboru = dokument.NazovSuboru;
                    dokument1.Skupina = (short)dokument.Skupina;
                    dokument1.DokumentPlatny = dokument.DokumentPlatny ?? "A";
                    dokument1.Poznamka = dokument.Poznamka ?? " ";// string.Empty;

                    DokumentDetail dokDetail1 = new DokumentDetail();
                    dokDetail1.NazovDokumentu = dokument.NazovDokumentu;
                    dokDetail1.Platny = dokument.DokumentPlatny ?? "A";
                    dokDetail1.Poznamka = string.Empty;
                    //TODO: ???
                    //dokDetail1.DocumentContent = System.IO.File.ReadAllBytes(myZakZO.ZakazkaTBdokument.FilePath);

                    dokument1.DokumentDetails.Add(dokDetail1);
                    newZakazka.Dokuments.Add(dokument1);
                }
            }
            return newZakazka;
        }
    }
}
