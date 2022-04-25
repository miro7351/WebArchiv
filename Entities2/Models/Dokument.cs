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
        public string? NazovDokumentu { get; set; }
        public string? NazovSuboru { get; set; }
        public short? Skupina { get; set; }
        public string? Poznamka { get; set; }

        public virtual Zakazka ZakazkaTgNavigation { get; set; } = null!;
        public virtual ICollection<DokumentDetail> DokumentDetails { get; set; }
    }
}
