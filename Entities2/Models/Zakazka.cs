using System.ComponentModel.DataAnnotations;

namespace PA.TOYOTA.DB
{

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
        public string ZakazkaTg { get; set; } = null!;


        [Display(Name = "Zák. TB")]
        [StringLength(8, MinimumLength = 0, ErrorMessage = "Zadajte údaj na max.8 znakov")]
        public string? ZakazkaTb { get; set; }


        [Display(Name = "Čís. prot.")]
        [StringLength(16, MinimumLength = 0, ErrorMessage = "Zadajte údaj na max.16 znakov")]
        public string? CisloProtokolu { get; set; }

        [Display(Name = "CWS")]
        [StringLength(16, MinimumLength = 0, ErrorMessage = "Zadajte údaj na max.16 znakov")]
        public string? Cws { get; set; }

        [StringLength(17, MinimumLength = 0, ErrorMessage = "Zadajte údaj na max.17 znakov")]
        [Display(Name = "Vin")]
        public string? Vin { get; set; }


        [Display(Name = "Ukončená")]
        public string? Ukoncena { get; set; } = null!;

        [StringLength(128, MinimumLength = 0, ErrorMessage = "Zadajte údaj na max.128 znakov")]
        [Display(Name = "Poznámka")]
        public string? Poznamka { get; set; }

        [StringLength(32, MinimumLength = 0, ErrorMessage = "Zadajte údaj na max.32 znakov")]
        [Display(Name = "Vytvoril")]
        public string? Vytvoril { get; set; }

        [DataType(DataType.Date)] //Na web stranke sa zobrazi Date control: Den mesiac rok
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]//Format pre vypis na stranke ak  sa pouzije @Html.DisplayFor(....), inac sa neformatuje!
        [Display(Name = "Vytvorené")]
        public DateTime? Vytvorene { get; set; }


        [StringLength(32, MinimumLength = 0, ErrorMessage = "Zadajte údaj na max.32 znakov")]
        [Display(Name = "Zmenil")]
        public string? Zmenil { get; set; }

        [DataType(DataType.Date)] //Na web stranke sa zobrazi Date control: Den mesiac rok
        [DisplayFormat(DataFormatString = "{0:dd.MM.yy HH:mm}", ApplyFormatInEditMode = true)]//Format pre vypis na stranke ak  sa pouzije @Html.DisplayFor(....), inac sa neformatuje!
        [Display(Name = "Zmenené")]
        public DateTime? Zmenene { get; set; }

        public virtual ICollection<Dokument> Dokuments { get; set; }
    }
}
