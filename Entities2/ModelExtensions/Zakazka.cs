using System.ComponentModel.DataAnnotations;


namespace PA.TOYOTA.DB
{
    //MH: 05.05.2022
  /*
   * Entity som presunul z adresara Entities2\Model  do Entities\MOdelExtensions
   */ 
    public partial class Zakazka
    {
        public Zakazka()
        {
            Dokuments = new HashSet<Dokument>();
        }

        public int ZakazkaId { get; set; }

        [Required]
        [StringLength(8, ErrorMessage = "Zadajte údaj na 8 znakov")]
        [Display(Name = "Zák. TG")]
        public string ZakazkaTg { get; set; } = null!;  //nchar(12), not null


        [Display(Name = "Zák. TB")]
        [StringLength(8, MinimumLength = 0, ErrorMessage = "Zadajte údaj na max.8 znakov")]
        public string? ZakazkaTb { get; set; }   //nchar(12), null


        [Display(Name = "Čís. prot.")]
        [StringLength(16, MinimumLength = 0, ErrorMessage = "Zadajte údaj na max.16 znakov")]
        public string? CisloProtokolu { get; set; }  //nchar(16), null

        [Display(Name = "Faktúra číslo")]
        [StringLength(16, MinimumLength = 0, ErrorMessage = "Zadajte údaj na max.16 znakov")]
        public string? CisloFaktury { get; set; }

        [Display(Name = "CWS")]
        [StringLength(16, MinimumLength = 0, ErrorMessage = "Zadajte údaj na max.16 znakov")]
        public string? Cws { get; set; }  //nchar(32), null

        [StringLength(17, MinimumLength = 0, ErrorMessage = "Zadajte údaj na max.17 znakov")]
        [Display(Name = "Vin")]
        public string? Vin { get; set; } //nchar(32), null

        [StringLength(16, MinimumLength = 0, ErrorMessage = "Zadajte údaj na max.16 znakov")]
        [Display(Name = "Číslo dielu")]
        public string? CisloDielu { get; set; } //nvarchar(16), null


        [Display(Name = "Ukončená")]
        public string? Ukoncena { get; set; } = null!;  //char(1) null

        [StringLength(16, MinimumLength = 0, ErrorMessage = "Zadajte údaj na max.16 znakov")]   //nvarchar(16), null
        [Display(Name = "Majiteľ")]
        public string? Majitel { get; set; }


        [StringLength(128, MinimumLength = 0, ErrorMessage = "Zadajte údaj na max.128 znakov")]
        [Display(Name = "Poznámka")]
        public string? Poznamka { get; set; }  //nvarchar(128), null

        [StringLength(32, MinimumLength = 0, ErrorMessage = "Zadajte údaj na max.32 znakov")]
        [Display(Name = "Vytvoril")]
        public string? Vytvoril { get; set; }//nvarchar(32), null  nastavuje sa v db table triggeri

        [DataType(DataType.Date)] //Na web stranke sa zobrazi Date control: Den mesiac rok
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]//Format pre vypis na stranke ak  sa pouzije @Html.DisplayFor(....), inac sa neformatuje!
        [Display(Name = "Vytvorené")]    //datetime2(0), null, nastavuje sa v db table triggeri
        public DateTime? Vytvorene { get; set; }


        [StringLength(32, MinimumLength = 0, ErrorMessage = "Zadajte údaj na max.32 znakov")]
        [Display(Name = "Zmenil")]
        public string? Zmenil { get; set; }  //nvarchar(32), null  nastavuje sa v db table triggeri

        [DataType(DataType.Date)] //Na web stranke sa zobrazi Date control: Den mesiac rok
        [DisplayFormat(DataFormatString = "{0:dd.MM.yy HH:mm}", ApplyFormatInEditMode = true)]//Format pre vypis na stranke ak  sa pouzije @Html.DisplayFor(....), inac sa neformatuje!
        [Display(Name = "Zmenené")]
        public DateTime? Zmenene { get; set; }  //datetime2(0), null, nastavuje sa v db table triggeri


        [StringLength(16, MinimumLength = 0, ErrorMessage = "Zadajte údaj na max.16 znakov")]
        [Display(Name = "ŠPZ")]
        public string? Spz { get; set; }  //nvarchar(16), null

       

        public virtual ICollection<Dokument> Dokuments { get; set; }
    }
}
