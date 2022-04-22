using System;
using System.Collections.Generic;

namespace PA.TOYOTA.DB
{
    public partial class DokumentDetail
    {
        public Guid DetailId { get; set; }
        public int DokumentId { get; set; }
        public short? Skupina { get; set; }
        public byte[]? DokumentContent { get; set; }
        public string? Vytvoril { get; set; }
        public DateTime? Vytvorene { get; set; }
        public string? Zmenil { get; set; }
        public DateTime? Zmenene { get; set; }

        public virtual Dokument Dokument { get; set; } = null!;
    }
}
