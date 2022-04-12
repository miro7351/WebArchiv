using PA.TOYOTA.DB;
using System.ComponentModel.DataAnnotations;

namespace ToyotaArchiv.Models
{
    //MH: 10.04.2022
    //ViewModel pre zobrazenie vybratych zakaziek od-do
    public class ZakazkyViewModel
    {
        public ZakazkyViewModel()
        {
            CurrentUser = new User() { UserRole = Infrastructure.USER_ROLE.None };
            StringList= new List<string>();
        }

        //datumy pre vyber poloziek z tab. Zakazka
        [DataType(DataType.Date)] //Na web stranke sa zobrazi Date control: Den mesiac rok
        [DisplayFormat(DataFormatString = "{0:dd.MM.yy HH:mm}")]//Format pre vypis na stranke ak  sa pouzije @Html.DisplayFor(....)
        [Display(Name = "Dátum od:")]
        public DateTime? DatumOd { get; set; }//starsi datum


        [DataType(DataType.Date)] //Na web stranke sa zobrazi Date control: Den mesiac rok
        [DisplayFormat(DataFormatString = "{0:dd.MM.yy HH:mm}")]//Format pre vypis na stranke ak  sa pouzije @Html.DisplayFor(....)
        [Display(Name = "Dátum do:")]
        public DateTime? DatumDo { get; set; }//novsi datum


        public User CurrentUser { get; set; }
        //public IEnumerable<Zakazka> Zakazky { get; set; }  toto je zle
        public IList<Zakazka> Zakazky { get; set; }
        public IList<string> StringList { get; set; }
    }
}
