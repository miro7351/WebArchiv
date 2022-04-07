using System;
using System.Collections.Generic;

namespace PA.TOYOTA.DB
{
    public partial class Zakazka
    {
        public Zakazka()
        {
            Dokuments = new HashSet<Dokument>();
        }

        public int ZakazkaId { get; set; }
        public string ZakazkaTg { get; set; } = null!;
        public string? ZakazkaTb { get; set; }
        public string? CisloProtokolu { get; set; }
        public string? Cws { get; set; }
        public string? Vin { get; set; }
        public string Platna { get; set; } = null!;
        public string Ukoncena { get; set; } = null!;
        public string Vytvoril { get; set; } = null!;
        public DateTime Vytvorene { get; set; }
        public string Zmenil { get; set; } = null!;
        public DateTime? Zmenene { get; set; }
        public string? Poznamka { get; set; }

        public virtual ICollection<Dokument> Dokuments { get; set; }
    }
}
