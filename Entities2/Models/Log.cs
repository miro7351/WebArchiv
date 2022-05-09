using System.ComponentModel.DataAnnotations;

namespace PA.TOYOTA.DB
{
    public partial class Log
    {
        public int LogId { get; set; }

        [DataType(DataType.Date)] //Na web stranke sa zobrazi Date control: Den mesiac rok
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm}")]//Format pre vypis na stranke ak  sa pouzije @Html.DisplayFor(....), inac sa neformatuje!
        [Display(Name = "Dátum")]    //datetime2(0), null, nastavuje sa v db table triggeri
        public DateTime LogDate { get; set; }

        [Display(Name = "Tabuľka")]
        public string? TableName { get; set; }

        [Display(Name = "Popis chyby")]
        public string? LogMessage { get; set; }

        [Display(Name = "Akcia")]
        public string? UserAction { get; set; }

        [Display(Name = "Užívateľ")]
        public string? UserName { get; set; }
    }
}
