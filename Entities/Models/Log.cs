
using System.ComponentModel.DataAnnotations;

namespace PA.TOYOTA.DB
{
    //MH: April 2022
    public partial class Log
    {
        [Display(Name = "ID")]
        public int LogId { get; set; }

        [DataType(DataType.Date)] //Na web stranke sa zobrazi Date control: Den mesiac rok
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm}")]//Format pre vypis na stranke ak  sa pouzije @Html.DisplayFor(....), inac sa neformatuje!
        [Display(Name="Dátum")]
        public DateTime LogDate { get; set; }

        [Display(Name = "Zdrojová tabuľka")]
        public string? TableName { get; set; }
        [Display(Name = "Správa")]
        public string? LogMessage { get; set; }

        [Display(Name = "Akcia")]
        public string? UserAction { get; set; }

        [Display(Name = "Užívateľ")]
        public string? UserName { get; set; }
    }
}
