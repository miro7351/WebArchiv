using System;
using System.Collections.Generic;

namespace PA.TOYOTA.DB
{
    public partial class DokumentDetail
    {
        public Guid DetailId { get; set; }
        public int DokumentId { get; set; }
        public string NazovDokumentu { get; set; } = null!;
        public string Platny { get; set; } = null!;
        public byte[] DocumentContent { get; set; } = null!;
        public string? Poznamka { get; set; }
        public string? Vytvoril { get; set; }
        public DateTime? Vytvorene { get; set; }
        public string? Zmenil { get; set; }
        public DateTime? Zmenene { get; set; }

        public virtual Dokument Dokument { get; set; } = null!;
    }
}
