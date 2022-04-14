
using System.Collections.ObjectModel;


namespace ToyotaArchiv.Domain
{
    //MH: Februar 2022
    /*
     * Typ pre popis zakazky;
     * 
     * ZakazkaZO: ZakazkaTG, ZakazkaTB, CWS, VIN, CisloProtokolu, Ukoncena, Platna, Poznamka, Vytvoril, Vytvorne, Zmenil, Zmenene;
     * 
     * Postup pri vytvoreni novej zakazky
     * 
     * 
     * A: Postup pri zobrazeni detailov o vybratej zakazke
     * 1, Vytvorit instanciu typu ZakazkaZO napr. MyZakazka
     * 
     * 2, Z DB nacitat udaje pre vybratu zakazku z tab. Zakazka (len jeden zaznam), Dokument ( udaje z tab. Dokument bude kolekcia zaznamov ) do instancie MyDbZakazka;
     * 2, Z  EF Entity MyDbZakazka skopirovat udaje do MyZakazka (ZakazkaTG, ZakazkaTB, CWS, VIN, CisloProtokolu, Ukoncena, Platna, Poznamka, Vytvoril, Vytvorne, Zmenil, Zmenene;)
     * 
     * 3. Entita MyDbZakazka obsahuje  ICollection<Dokument> Dokuments { get; set; }
     * 
     *    Do MyZakazka.ZakazkaTGdocument skopirovat prislusne udaje z MyDbZakazka.Dokuments.FirstOrDefault( p => p.Skupina==0 );
     *    Do MyZakazka.ZakazkaTBdocument skopirovat prislusne udaje   MyDbZakazka.Dokuments.FirstOrDefault( p => p.Skupina==1 );
     * 
     * 
     *  Do MyZakazka.PovinneDokumenty skopirovat prislusne udaje  ... MyDbZakazka.Dokuments.Where( p => p.Skupina>=20 && p.Skupina< 100 );
     *  Do MyZakazka.Prilohy skopirovat prislusne udaje  ... MyDbZakazka.Dokuments.Where( p => p.Skupina>=100 );
     * 
     * -----------------------------------------------------------------------------
     * 
     *  * B: Postup pri zobrazeni detailov a editacii udajov pre  vybratu zakazku
     * 1, Vytvorit instanciu typu ZakazkaZO napr. MyZakazka
     * 
     * 2, Z DB nacitat udaje pre vybratu zakazku z tab. Zakazka (len jeden zaznam), Dokument ( udaje z tab. Dokument bude kolekcia zaznamov ) do instancie MyDbZakazka;
     * 2, Z  EF Entity MyDbZakazka skopirovat udaje do MyZakazka (ZakazkaTG, ZakazkaTB, CWS, VIN, CisloProtokolu, Ukoncena, Platna, Poznamka, Vytvoril, Vytvorne, Zmenil, Zmenene;)
     * 
     * 3. Entita MyDbZakazka obsahuje  ICollection<Dokument> Dokuments { get; set; }
     * 
     *    Do MyZakazka.ZakazkaTGdocument skopirovat prislusne udaje z MyDbZakazka.Dokuments.FirstOrDefault( p => p.Skupina==0 );
     *    Do MyZakazka.ZakazkaTBdocument skopirovat prislusne udaje   MyDbZakazka.Dokuments.FirstOrDefault( p => p.Skupina==1 );
     * 
     * 
     *  Do MyZakazka.PovinneDokumenty skopirovat prislusne udaje  ... MyDbZakazka.Dokuments.Where( p => p.Skupina>=20 && p.Skupina< 100 );
     *  Do MyZakazka.Prilohy skopirovat prislusne udaje  ... MyDbZakazka.Dokuments.Where( p => p.Skupina>=100 );
     * 
     * 4, Udaje z MyZakazka prepisat do MyDbZakazka
     * 5, Ulozit udaje do DB.
     * 
     */
    /// <summary>
    /// Typ pre uplny popis zarucnej opravy;
    /// </summary>
    public class ZakazkaZO  //Zakazka Zarucnej Opravy
    {
        public ZakazkaZO()
        {
         
            ZakazkaTGdokument = new BaseItem();//NazovDokumentu=ZakazkaTG, Skupina=1;
            ZakazkaTBdokument = new BaseItem();//NazovDokumentu=ZakazkaTB, Skupina=2;

            PovinneDokumenty = new ObservableCollection<BaseItem>();
            Prilohy = new ObservableCollection<BaseItem>();
        }

        #region ==Properties==


        //zaznam z tab. Dokument, Skupina=1   NazovDokumentu=ZakazkaTG
        public BaseItem ZakazkaTGdokument //polozka z kolekcie Dokumenty kde Skupina=1
        {
            get;
            set;
        }

        //zaznam z tab. Dokument, Skupina=2   NazovDokumentu=ZakazkaTB
        public BaseItem ZakazkaTBdokument //polozka z kolekcie Dokumenty kde Skupina=2
        {
            get;
            set;
        }
        public IList<BaseItem> PovinneDokumenty //Skupina=20,21,,..,99; nemoze obsahovat 2 polozky  s rovnakym Name, je to osetrene??
        {
            get;
            set;
        }

        //zaznamy z tab. Documenty
        /// <summary>
        /// Zoznam priloh k zarucnej oprave
        /// </summary>

        public IList<BaseItem> Prilohy //Skupina=100,101,,..; nemoze obsahovat 2 polozky  s rovnakym Name, je to osetrene??
        {
            get;
            set;
        }

      
        /// <summary>
        /// Vznika v momente prijmu vozidla do servisu kedy este nie je jasne ci ide o zarucnu opravu
        /// Identifikator v ramci predajne pre garancnu opravu;
        /// </summary>
        public string ZakazkaTb
        {
            get;
            set;
        }

      
        /// <summary>
        /// Zakazka Toyota Garancia;
        /// Identifikator v ramci predajne pre garancnu opravu;
        /// </summary>
        public string ZakazkaTg
        {
            get;
            set;
        }

     
        /// <summary>
        /// Kod opravy auta z europskej databazy
        /// </summary>
        public string Cws
        {
            get;
            set;
        }

       
        /// <summary>
        /// Udaj znamy po schvaleni garancnej opravy od importera;
        /// </summary>
        public string CisloProtokolu
        {
            get;
            set;
        }


      
        /// <summary>
        /// VIN kod z tech. preukazu automobilu
        /// </summary>
        public string Vin
        {
            get;
            set;
        }

       
        /// <summary>
        /// Stav zakazky;
        /// "A" - zakazka je ukoncena, "N" - zakazka este nie je ukoncena, neobsahuje este vsetky subory;
        /// Ak sa zada posledny povinny subor, automaticky sa nastavi Ukoncena="A";
        /// </summary>
        public string Ukoncena //Ukoncena A/N
        {
            get;
            set;
        }


        /* TODO: ???? Nepouzit Temporal table pre MS SQL Server????
         * MH: Udaje z db tab. sa nebudu dat mazat, lebo je to archivny system, preto akoby pre vymazany zaznam nastavim flag PlatnyZaznam="N", taky zaznam sa len nebude zobrazovat;
         */


        //string platna  A/N
     
        /// <summary>
        /// Priznak ci je to platny zaznam;
        /// "A" -platny zaznam sa zobrazuje, "N" -neplatny nebude sa normalne zobrazovat, ale v tab. bude existovat;
        /// </summary>
        public string Platna    //Platna A/N
        {
            get;
            set;
        }


      
        /// <summary>
        /// Poznamka nvarchar(128), null
        /// </summary>
        public string Poznamka   
        {
            get;
            set;
        }

        public string Vytvoril { get; set; }
        public DateTime? Vytvorene { get; set; }
        public string Zmenil { get; set; }
        public DateTime? Zmenene { get; set; }

        #endregion ==Properties==

        /*
         * DB ma 3 hlavne tabulky:
         * Zakazka: ZakazkaID, ZakazkaTG, ZakazkaTB, VIN, CWS, CisloProtokolu, Platna, Ukoncena, Poznamka, Vytvoril, Vytvorene, Zmenil, Zmenene, vsetko su to len jednoduche typy nvarchar(xx)
         * 
         * Dokument: subory pre zakazku, povinne dokumenty a prilohy; subory typu: *.pdf, *.txt, *.jpeg,....
         *           DokumentID, ZakazkaTG, NazovDokumentu, NazovSuboru, DokumentPlatny, Skupina, Poznamka, Vytvoril, Vytvorene, Zmenil, Zmenene
         * Document_Detail:  DokumentID, NazovDokumentu, DocumentContent,  Platny, Poznamka,  Vytvoril, Vytvorene, Zmenil, Zmenene          
         */


    }//class 
}
