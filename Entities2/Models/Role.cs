using System.ComponentModel.DataAnnotations;

namespace PA.TOYOTA.DB
{

    //MH: april 2022
    //MH: zaznamy v tab. Role sa nedaju mazat!!!
    //Admin, alebo VEDUCI moze menit len Aktivny; V DB sa moze robit INSERT
    public partial class Role
    {

        [Display(Name = "Rola ID")]
        public short RoleId { get; set; }   //nastavuje admin; smallint

        [StringLength(24, MinimumLength = 5, ErrorMessage = "Zadajte údaj od 5 do max.24 znakov")]
        [Display(Name = "Názov roly")]
        public string RoleName { get; set; } = null!;   //nchar(24)

        [Display(Name = "Aktívna")]
        public string Aktivny { get; set; } = null!;  // A/N

        [Display(Name = "Vytvoril")]
        public string? Vytvoril { get; set; }  //nvarchar(32)   //nastavuja sa v DB

        [Display(Name = "Vytvorené")]
        [DataType(DataType.Date)] //Na web stranke sa zobrazi Date control: Den mesiac rok
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]//Format pre vypis na stranke ak  sa pouzije @Html.DisplayFor(....), inac sa neformatuje!
        public DateTime? Vytvorene { get; set; }  //nastavuja sa v DB

        [Display(Name = "Zmenil")]           //nastavuja sa v DB
        public string? Zmenil { get; set; }//nastavuja sa v DB

        [Display(Name = "Zmenené")]
        [DataType(DataType.Date)] //Na web stranke sa zobrazi Date control: Den mesiac rok
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]//Format pre vypis na stranke ak  sa pouzije @Html.DisplayFor(....), inac sa neformatuje!
        public DateTime? Zmenene { get; set; }//nastavuja sa v DB

        [StringLength(64, MinimumLength = 0, ErrorMessage = "Zadajte údaj na max.64 znakov")]
        [Display(Name = "Poznámka")]
        public string? Poznamka { get; set; }  //nvarchar(64)
    }
}
