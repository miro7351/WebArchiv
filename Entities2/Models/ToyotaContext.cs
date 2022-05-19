using Microsoft.EntityFrameworkCore;

namespace PA.TOYOTA.DB
{
    public partial class ToyotaContext : DbContext
    {
       

        public virtual DbSet<Account> Accounts { get; set; } = null!;
        public virtual DbSet<Dokument> Dokuments { get; set; } = null!;
        public virtual DbSet<DokumentDetail> DokumentDetails { get; set; } = null!;
        public virtual DbSet<Error> Errors { get; set; } = null!;
        public virtual DbSet<Log> Logs { get; set; } = null!;
        public virtual DbSet<Zakazka> Zakazkas { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=HRABCAK;Database=TOYOTA_T1;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasKey(e => e.LoginName);

                entity.ToTable("Account");

                entity.Property(e => e.LoginName)
                    .HasMaxLength(16)
                    .IsUnicode(false);

                entity.Property(e => e.DbLogin).HasMaxLength(16);

                entity.Property(e => e.DbPassword).HasMaxLength(16);

                entity.Property(e => e.LoginPassword)
                    .HasMaxLength(16)
                    .IsUnicode(false);

                entity.Property(e => e.LoginRola)
                    .HasMaxLength(16)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Dokument>(entity =>
            {
                entity.ToTable("Dokument");

                entity.Property(e => e.DokumentId).HasColumnName("DokumentID");

                entity.Property(e => e.NazovDokumentu)
                    .HasMaxLength(64)
                    .IsFixedLength();

                entity.Property(e => e.NazovSuboru)
                    .HasMaxLength(64)
                    .IsFixedLength();

                entity.Property(e => e.Poznamka).HasMaxLength(128);

                entity.Property(e => e.ZakazkaTg)
                    .HasMaxLength(12)
                    .HasColumnName("ZakazkaTG")
                    .IsFixedLength();

                entity.HasOne(d => d.ZakazkaTgNavigation)
                    .WithMany(p => p.Dokuments)
                    .HasForeignKey(d => d.ZakazkaTg)
                    .HasConstraintName("FK_Dokument_Zakazka");
            });

            modelBuilder.Entity<DokumentDetail>(entity =>
            {
                entity.HasKey(e => e.DetailId)
                    .HasName("PK_Dokument_Detail1");

                entity.ToTable("Dokument_Detail");

                entity.Property(e => e.DetailId)
                    .HasColumnName("DetailID")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.DokumentId).HasColumnName("DokumentID");

                entity.HasOne(d => d.Dokument)
                    .WithMany(p => p.DokumentDetails)
                    .HasForeignKey(d => d.DokumentId)
                    .HasConstraintName("FK_Dokument_Detail_Dokument");
            });

            modelBuilder.Entity<Error>(entity =>
            {
                entity.HasKey(e => e.ErrorLogId);

                entity.ToTable("Error");

                entity.Property(e => e.ErrorLogId).HasColumnName("ErrorLogID");

                entity.Property(e => e.ErrorDate)
                    .HasPrecision(0)
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ErrorMsg)
                    .HasMaxLength(256)
                    .IsUnicode(false);

                entity.Property(e => e.ErrorProcedure)
                    .HasMaxLength(256)
                    .IsUnicode(false);

                entity.Property(e => e.User)
                    .HasMaxLength(32)
                    .HasDefaultValueSql("(user_name())");
            });

            modelBuilder.Entity<Log>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Datum).HasPrecision(0);

                entity.Property(e => e.NovaHodnota)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("Nova_hodnota");

                entity.Property(e => e.Operacia)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Parameter)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PovodnaHodnota)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("Povodna_hodnota");

                entity.Property(e => e.TgZakazka)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("TG_Zakazka");

                entity.Property(e => e.Uzivatel)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Zakazka>(entity =>
            {
                entity.HasKey(e => e.ZakazkaTg);

                entity.ToTable("Zakazka");

                entity.Property(e => e.ZakazkaTg)
                    .HasMaxLength(12)
                    .HasColumnName("ZakazkaTG")
                    .IsFixedLength();

                entity.Property(e => e.CisloDielu).HasMaxLength(16);

                entity.Property(e => e.CisloFaktury)
                    .HasMaxLength(16)
                    .IsFixedLength();

                entity.Property(e => e.CisloProtokolu)
                    .HasMaxLength(16)
                    .IsFixedLength();

                entity.Property(e => e.Cws)
                    .HasMaxLength(32)
                    .HasColumnName("CWS")
                    .IsFixedLength();

                entity.Property(e => e.Majitel).HasMaxLength(64);

                entity.Property(e => e.Poznamka).HasMaxLength(128);

                entity.Property(e => e.Spz)
                    .HasMaxLength(16)
                    .HasColumnName("SPZ");

                entity.Property(e => e.Ukoncena)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('N')")
                    .IsFixedLength();

                entity.Property(e => e.Vin)
                    .HasMaxLength(17)
                    .HasColumnName("VIN")
                    .IsFixedLength();

                entity.Property(e => e.Vytvorene).HasPrecision(0);

                entity.Property(e => e.Vytvoril).HasMaxLength(32);

                entity.Property(e => e.ZakazkaId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ZakazkaID");

                entity.Property(e => e.ZakazkaTb)
                    .HasMaxLength(12)
                    .HasColumnName("ZakazkaTB")
                    .IsFixedLength();

                entity.Property(e => e.Zmenene).HasPrecision(0);

                entity.Property(e => e.Zmenil).HasMaxLength(32);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
