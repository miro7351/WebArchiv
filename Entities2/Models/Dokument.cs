
using System.ComponentModel.DataAnnotations;

namespace PA.TOYOTA.DB
{
    //MH: 21.04.2022
    public partial class Dokument
    {
        public Dokument()
        {
            DokumentDetails = new HashSet<DokumentDetail>();
        }

        public int DokumentId { get; set; }
        public string ZakazkaTg { get; set; } = null!;

        [Display(Name = "NázovDokumentu")]
        public string? NazovDokumentu { get; set; }

        [Display(Name = "NázovSuboru")]
        public string? NazovSuboru { get; set; }
        public short? Skupina { get; set; }

        [Display(Name = "Poznámka")]
        public string? Poznamka { get; set; }



        public string? Vytvoril { get; set; }

        [DataType(DataType.Date)] //Na web stranke sa zobrazi Date control: Den mesiac rok
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}")]//Format pre vypis na stranke ak  sa pouzije @Html.DisplayFor(....), inac sa neformatuje!
        [Display(Name = "Vytvorené")]
        public DateTime? Vytvorene { get; set; }
        public string? Zmenil { get; set; }



        [DataType(DataType.Date)] //Na web stranke sa zobrazi Date control: Den mesiac rok
        [DisplayFormat(DataFormatString = "{0:dd.MM.yy HH:mm}")]//Format pre vypis na stranke ak  sa pouzije @Html.DisplayFor(....), inac sa neformatuje!
        [Display(Name = "Zmenené")]
        public DateTime? Zmenene { get; set; }

        public virtual Zakazka ZakazkaTgNavigation { get; set; } = null!;
        public virtual ICollection<DokumentDetail> DokumentDetails { get; set; }
    }
}
