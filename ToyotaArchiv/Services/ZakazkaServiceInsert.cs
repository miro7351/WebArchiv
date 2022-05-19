using ToyotaArchiv.Domain;
using PA.TOYOTA.DB;
using ToyotaArchiv.Infrastructure;

namespace ToyotaArchiv.Services
{
    //MH: april 2022
    //pozri aj ZakazkaService.cs
    internal partial class ZakazkaServiceWeb : IZakazkaTransformService
    {

        /// <summary>
        /// Typ instancie ZakazkaZO skonveruje do NOVEJ instancie Typu Zakazka;
        /// Ak sa udaje  zadane uzivatelom maju INSERTOVAT do databazy;
        /// </summary>
        /// <param name="zakazkaZO">Instancia editovana uzivatelom</param>
        /// <returns>Instancia, ktora sa ma zapisat do databazy</returns>
        public Zakazka ConvertZakazkaZO_To_NewZakazka(ref ZakazkaZO zakazkaZO)  //Ak sa vytvara nova instancia typu Zakazka pre INSERT do DB!!!
        {
            if (zakazkaZO == null)
            {
                throw new ArgumentNullException($"Chyba-ConvertZakazkaZO_To_Zakazka: Neplatný údaj pre vstupnú zákazku!");
            }
            Zakazka newZakazkaDB = new Zakazka();

            newZakazkaDB.ZakazkaTg = zakazkaZO.ZakazkaTg.Trim();
            newZakazkaDB.ZakazkaTb = zakazkaZO?.ZakazkaTb?.Trim();
            newZakazkaDB.Poznamka = zakazkaZO?.Poznamka?.Trim();
            newZakazkaDB.Ukoncena = zakazkaZO?.Ukoncena;
            newZakazkaDB.Vin = zakazkaZO?.Vin?.Trim();
            newZakazkaDB.Cws = zakazkaZO?.Cws?.Trim();
            newZakazkaDB.Spz = zakazkaZO?.SPZ?.Trim();
            newZakazkaDB.Majitel = zakazkaZO?.Majitel?.Trim();
            newZakazkaDB.CisloProtokolu = zakazkaZO?.CisloProtokolu?.Trim();
            newZakazkaDB.CisloFaktury = zakazkaZO?.CisloFaktury?.Trim();
            newZakazkaDB.CisloDielu = zakazkaZO?.CisloDielu?.Trim();

            newZakazkaDB.Vytvoril = zakazkaZO?.Vytvoril?.Trim();
            newZakazkaDB.Vytvorene = zakazkaZO?.Vytvorene;


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
                if (zakazkaZO.ZakazkaTBdokument.FileContent != null)
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
                if (povinneDokumentyZO != null)
                {
                    if (povinneDokumentyZO.Count == PocetPovinnych)
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
                        if (povinnyDokumentZO.FileContent != null)  //FileContent pole bytov, obsahuje obrazok z klienta;
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

                        if (prilohaZO.FileContent != null) //FileContent pole bytov, obsahuje obrazok z klienta;
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



    }
}
