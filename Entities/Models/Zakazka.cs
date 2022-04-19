using System;
using System.Collections.Generic;
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

        [Display(Name = "Zák. TG")]
        public string ZakazkaTg { get; set; } = null!;

        [Display(Name = "Zák. TB")]
        public string? ZakazkaTb { get; set; }

        [Display(Name = "Čís. prot.")]
        public string? CisloProtokolu { get; set; }
        public string? Cws { get; set; }

        [Display(Name = "Vin")]
        public string? Vin { get; set; }

        [Display(Name = "Platná")]
        public string Platna { get; set; } = null!;


        [Display(Name = "Ukončená")]
        public string Ukoncena { get; set; } = null!;

        [StringLength(32, MinimumLength = 0, ErrorMessage = "Zadajte údaj na max.32 znakov")]
        [Display(Name = "Vytvoril")]
        public string Vytvoril { get; set; }  //pri editovani je to readonly!!-

        //public string Vytvoril { get; set; } = null!;  //pri editovani je to readonly!!-

        [DataType(DataType.Date)] //Na web stranke sa zobrazi Date control: Den mesiac rok
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}")]//Format pre vypis na stranke ak  sa pouzije @Html.DisplayFor(....), inac sa neformatuje!
        [Display(Name = "Vytvorené")]
        public DateTime Vytvorene { get; set; }  //pri editovani je to readonly!!


        [StringLength(32, MinimumLength = 0, ErrorMessage = "Zadajte údaj na max.32 znakov")]
        [Display(Name = "Zmenil")]
        public string Zmenil { get; set; } //pri editovani je to readonly!!
        //public string Zmenil { get; set; } = null!; //pri editovani je to readonly!!

        [DataType(DataType.Date)] //Na web stranke sa zobrazi Date control: Den mesiac rok
        [DisplayFormat(DataFormatString = "{0:dd.MM.yy HH:mm}")]//Format pre vypis na stranke ak  sa pouzije @Html.DisplayFor(....), inac sa neformatuje!
        [Display(Name = "Zmenené")]
        public DateTime? Zmenene { get; set; }  //pri editovani je to readonly!!

        [Display(Name = "Poznámka")]
        public string? Poznamka { get; set; }

        public virtual ICollection<Dokument> Dokuments { get; set; }
    }
}
