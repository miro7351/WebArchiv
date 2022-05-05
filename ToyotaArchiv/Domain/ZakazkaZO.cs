
using System.ComponentModel.DataAnnotations;

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
    public class ZakazkaZO  //Zakazka Zarucnej Opravy; je to ViewModel pre typ Zakazka
    {
        /// <summary>
        /// Vytvori PovinneDokumenty, Prilohy, ZakazkaTGdokument a ZakazkaTBdokument
        /// </summary>
        public ZakazkaZO()
        {
            ZakazkaTGdokument = new BaseItem() { Skupina=1 };//NazovDokumentu=ZakazkaTG, Skupina=1;
            ZakazkaTBdokument = new BaseItem() { Skupina=2 };//NazovDokumentu=ZakazkaTB, Skupina=2;

            PovinneDokumenty = new List<BaseItem>(); //Skupina: 20,21,22,...,99
            Prilohy = new List<BaseItem>();          //Skupina: 100, 101,.....
            ErrorMessage = string.Empty;
        }


        #region ==Properties==


        //zaznam z tab. Dokument, Skupina=1   NazovDokumentu=ZakazkaTG
        public BaseItem? ZakazkaTGdokument //polozka z kolekcie Dokumenty kde Skupina=1
        {
            get;
            set;
        }

        //zaznam z tab. Dokument, Skupina=2   NazovDokumentu=ZakazkaTB
        public BaseItem? ZakazkaTBdokument //polozka z kolekcie Dokumenty kde Skupina=2
        {
            get;
            set;
        }
        public IList<BaseItem>? PovinneDokumenty //Skupina=20,21,,..,99; NazovDokumentu v polozke je pevne dany, nemoze sa menit;
        {
            get;
            set;
        }

        //zaznamy z tab. Documenty
        /// <summary>
        /// Zoznam priloh k zarucnej oprave
        /// </summary>
        public IList<BaseItem>? Prilohy //Skupina=100,101,102,..;  NazovDokumentu je v polozke prednastaveny na Prilohaxx, uzivatel ho moze menit;
        {
            get;
            set;
        }

        /// <summary>
        /// Zakazka Toyota Garancia;
        /// Identifikator v ramci firmy pre garancnu opravu;
        /// </summary>
        /// [
        /// 

        [Required(ErrorMessage = "Údaj ZakazkaTg je povinný.")] //OK ak sa nezada nic a spusti sa Submit
        [Display(Name = "ZákazkaTG")]             //OK
        [StringLength(8, MinimumLength=8, ErrorMessage = "Pre údaj ZakazkaTg zadajte 8 znakov.")]//Ak sa nezada MinimumLength=8; nejdeto
        public string ZakazkaTg
        {
            get;
            set;
        }
        
       

        /// <summary>
        /// Vznika v momente prijmu vozidla do servisu kedy este nie je jasne ci ide o zarucnu opravu
        /// Identifikator v ramci predajne pre garancnu opravu; Nemusi za zadat pri vytvoreni zakazkyô
        /// </summary>
        [Display(Name = "ZákazkaTB")]
        [StringLength(8, MinimumLength = 8, ErrorMessage = "Pre údaj ZakazkaTb zadajte 8 znakov.")]//nemusi sa zadat, ale ak sa zada, potom to musi byt 8 znakov;
        public string? ZakazkaTb
        {
            get;
            set;
        }

        /// <summary>
        /// Kod opravy auta z europskej databazy
        /// </summary>
        [StringLength(20, MinimumLength = 0, ErrorMessage = "Pre údaj CWS zadajte max.20 znakov.")]//nemusi sa zadat, ale ak sa zada, potom to musi byt max. 20 znakov;
        [Display(Name = "CWS")]
        public string? Cws
        {
            get;
            set;
        }


        /// <summary>
        /// Udaj znamy po schvaleni garancnej opravy od importera;
        /// </summary>
        [Display(Name = "Číslo protokolu")]
        [StringLength(16, MinimumLength = 0, ErrorMessage = "Pre údaj CisloProtokolu zadajte max. 16 znakov.")]//nemusi sa zadat, ale ak sa zada, potom to musi byt max. 16 znakov;
        public string? CisloProtokolu
        {
            get;
            set;
        }

        /// <summary>
        /// VIN kod z tech. preukazu automobilu, 17 znakov
        /// </summary>
        [StringLength(17, MinimumLength = 17, ErrorMessage = "Pre údaj VIN zadajte 17 znakov.")]//nemusi sa zadat, ale ak sa zada, potom to musi byt 17 znakov;
        [Display(Name = "VIN")]
        public string? Vin
        {
            get;
            set;
        }

        [StringLength(16, MinimumLength = 0, ErrorMessage = "Zadajte údaj na max.16 znakov")]
        [Display(Name = "ŠPZ")]
        public string? SPZ { get; set; }  //nvarchar(16), null

        [StringLength(64, MinimumLength = 0, ErrorMessage = "Zadajte údaj na max.64 znakov")]
        [Display(Name = "Vlastník vozidla")]
        public string? Vlastnik { get; set; } //nvarchar(32), null


        /// <summary>
        /// Stav zakazky;
        /// "A" - zakazka je ukoncena, "N" - zakazka este nie je ukoncena, neobsahuje este vsetky subory;
        /// Ak sa zada posledny povinny subor, automaticky sa nastavi Ukoncena="A";
        /// </summary>
        [Display(Name = "Ukončená")]
        [StringLength(1, ErrorMessage = "Zadajte údaj na A alebo N")]
        public string? Ukoncena //Ukoncena A/N  needituje sa!!
        {
            get;
            set;
        }


        /* TODO: ???? Nepouzit Temporal table pre MS SQL Server????
         * MH: Udaje z db tab. sa nebudu dat mazat, lebo je to archivny system, preto akoby pre vymazany zaznam nastavim flag PlatnyZaznam="N", taky zaznam sa len nebude zobrazovat;
         */
        /// <summary>
        /// Poznamka nvarchar(128), null
        /// </summary>
        [Display(Name = "Poznámka")]
        [StringLength(128, MinimumLength = 0, ErrorMessage = "Pre údaj Poznamka zadajte max. 128 znakov.")]
        public string? Poznamka   
        {
            get;
            set;
        }

        [Display(Name = "Vytvoril")]
        public string? Vytvoril { get; set; }
        
        [DataType(DataType.Date)] //Na web stranke sa zobrazi Date control: Den mesiac rok
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm}")]//Format pre vypis na stranke ak  sa pouzije @Html.DisplayFor(....), inac sa neformatuje!
        [Display(Name = "Vytvorené")]
        public DateTime? Vytvorene { get; set; }


        [StringLength(32, MinimumLength = 0, ErrorMessage = "Zadajte údaj na max.32 znakov")]
        [Display(Name = "Zmenil")]
        public string? Zmenil { get; set; }

        [DataType(DataType.Date)] //Na web stranke sa zobrazi Date control: Den mesiac rok
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm}")]//Format pre vypis na stranke ak  sa pouzije @Html.DisplayFor(....), inac sa neformatuje!
        [Display(Name = "Zmenené")]
        public DateTime? Zmenene { get; set; }


        public string? ErrorMessage { get; set; }  //pre zobrazenie chyby, ktora nastane pri spracovani, alebo ukladani do DB


        #endregion ==Properties==

        /*
         * DB ma 3 hlavne tabulky:
         * Zakazka: ZakazkaID, ZakazkaTG, ZakazkaTB, VIN, CWS, CisloProtokolu, Platna, Ukoncena, Poznamka, Vytvoril, Vytvorene, Zmenil, Zmenene, vsetko su to len jednoduche typy nvarchar(xx)
         * 
         * Dokument: subory pre zakazku, povinne dokumenty a prilohy; subory typu: *.pdf, *.txt, *.jpeg,....
         *           DokumentID, ZakazkaTG, Skupina, NazovDokumentu, NazovSuboru, Skupina, Poznamka
         * Document_Detail:  DokumentID, Skupina, DocumentContent          
         */


    }//class 
}
