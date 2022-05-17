using System.ComponentModel.DataAnnotations;

namespace PA.TOYOTA.DB
{
    //LV 17.05.2022
    //Logy z tabulky Zakazka
    public partial class Log1
    {
        public long Id { get; set; }

        [DataType(DataType.Date)]   
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm}")]//Format pre vypis na stranke ak  sa pouzije @Html.DisplayFor(....), inac sa neformatuje!
        [Display(Name = "Dátum")]    //datetime2(0), null, nastavuje sa v db table triggeri
        public DateTime? Datum { get; set; }

        [Display(Name = "ZakazkaTG")]
        public string? TgZakazka { get; set; }

        [Display(Name = "Operácia")]
        public string? Operacia { get; set; }

        [Display(Name = "Parameter")]
        public string? Parameter { get; set; }

        [Display(Name = "Pôvodná hodnota")]
        public string? PovodnaHodnota { get; set; }

        [Display(Name = "Nová hodnota")]
        public string? NovaHodnota { get; set; }

        [Display(Name = "Užívateľ")]
        public string? Uzivatel { get; set; }
    }
}
