using System.ComponentModel.DataAnnotations;

namespace PA.TOYOTA.DB
{
    public partial class Error
    {
        public int ErrorLogId { get; set; }

        [DataType(DataType.Date)] //Na web stranke sa zobrazi Date control: Den mesiac rok
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm}")]//Format pre vypis na stranke ak  sa pouzije @Html.DisplayFor(....), inac sa neformatuje!
        [Display(Name = "Dátum")]    //datetime2(0), null, nastavuje sa v db table triggeri
        public DateTime ErrorDate { get; set; }

        [Display(Name = "Popis chyby")]
        public string? ErrorMsg { get; set; }
        [Display(Name = "Číslo chyby")]
        public int? ErrorNumber { get; set; }

        [Display(Name = "Procedúra")]
        public string? ErrorProcedure { get; set; }

        [Display(Name = "Riadok")]
        public int? ErrorLine { get; set; }

        [Display(Name = "Užívateľ")]
        public string? User { get; set; }
    }
}
