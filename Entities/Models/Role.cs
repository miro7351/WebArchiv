using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PA.TOYOTA.DB
{
    public partial class Role
    {
        
        public short RoleId { get; set; }
        
        [Display(Name ="Rola")]
        public string RoleName { get; set; } = null!;


        [Display(Name = "Aktívny")]
        public string Aktivny { get; set; } = null!;

        [Display(Name = "Vytvoril")]
        public string? Vytvoril { get; set; }


        [Display(Name = "Vytvorené")]
        public DateTime? Vytvorene { get; set; }

       [Display(Name = "Zmenil")]
        public string? Zmenil { get; set; }

        [Display(Name = "Zmenené")]
        public DateTime? Zmenene { get; set; }

        [Display(Name = "Poznámka")]
        public string? Poznamka { get; set; }
    }
}
