using System;
using System.Collections.Generic;

namespace PA.TOYOTA.DB
{
    public partial class Dokument
    {
        public Dokument()
        {
            DokumentDetails = new HashSet<DokumentDetail>();
        }

        public int DokumentId { get; set; }
        public string ZakazkaTg { get; set; } = null!;
        public string NazovDokumentu { get; set; } = null!;
        public string NazovSuboru { get; set; } = null!;
        public string DokumentPlatny { get; set; } = null!;
        public short Skupina { get; set; }
        public string Vytvoril { get; set; } = null!;
        public DateTime Vytvorene { get; set; }
        public string Zmenil { get; set; } = null!;
        public DateTime? Zmenene { get; set; }
        public string Poznamka { get; set; } = null!;

        public virtual Zakazka ZakazkaTgNavigation { get; set; } = null!;
        public virtual ICollection<DokumentDetail> DokumentDetails { get; set; }
    }
}
