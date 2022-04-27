using System.ComponentModel.DataAnnotations;
using ToyotaArchiv.Infrastructure;

namespace ToyotaArchiv.Models
{
    //MH: 10.04.2022
    //Parametre pre uzivatela aplikacie
    public class User
    {
        [Display(Name = "Užívateľ")]
        public string UserName { get; set; }

        [Display(Name = "Heslo")]
        public string Password { get; set; }

        [Display(Name = "Login")]
        public string Login { get; set; }

        [Display(Name = "Aktívny")]
        public bool Enabled { get; set; }

        [Display(Name = "email")]
        public string Email { get; set; }

        [Display(Name = "Rola")]
        //public USER_ROLE UserRole { get; set; }
        public string UserRole { get; set; }

    }
}
