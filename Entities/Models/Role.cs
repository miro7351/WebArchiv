using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PA.TOYOTA.DB
{
    /*
     Pre rolu da moze menit len property Aktivny. Moze to menit Admin, Veduci;
     Pri editacii je viditelne len RoleName, Aktivny, Poznamka;
    Ostatne polozky su neviditelne.
    Pri zmene Aktivny sa zapisuje message do tab. Logs;
    V DB tab. Roles sa nedaju mazat zaznamy.

     */
    public partial class Role
    {
        
        public short RoleId { get; set; }
        
        [Display(Name ="Rola")]
        public string RoleName { get; set; } = null!;


        [Display(Name = "Aktívny")]
        public string Aktivny { get; set; } = null!;

        [Display(Name = "Vytvoril")]
        public string? Vytvoril { get; set; }

       
        [DataType(DataType.Date)] //Na web stranke sa zobrazi Date control: Den mesiac rok
        [DisplayFormat(DataFormatString = "{0:dd.MM.yy HH:mm}")]//Format pre vypis na stranke ak  sa pouzije @Html.DisplayFor(....), inac sa neformatuje!
        [Display(Name = "Vytvorené")]
        public DateTime? Vytvorene { get; set; }

       [Display(Name = "Zmenil")]
        public string? Zmenil { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yy HH:mm}")]
        [Display(Name = "Zmenené")]
        public DateTime? Zmenene { get; set; }

        [Display(Name = "Poznámka")]
        public string? Poznamka { get; set; }
    }
}
