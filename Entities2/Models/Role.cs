using System;
using System.Collections.Generic;

namespace PA.TOYOTA.DB
{
    public partial class Role
    {
        public short RoleId { get; set; }
        public string RoleName { get; set; } = null!;
        public string Aktivny { get; set; } = null!;
        public string? Vytvoril { get; set; }
        public DateTime? Vytvorene { get; set; }
        public string? Zmenil { get; set; }
        public DateTime? Zmenene { get; set; }
        public string? Poznamka { get; set; }
    }
}
