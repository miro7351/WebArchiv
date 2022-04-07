using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace PA.TOYOTA.DB
{
    public partial class ToyotaContext : DbContext
    {
        public ToyotaContext()
        {
        }

        public ToyotaContext(DbContextOptions<ToyotaContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Dokument> Dokuments { get; set; } = null!;
        public virtual DbSet<DokumentDetail> DokumentDetails { get; set; } = null!;
        public virtual DbSet<Error> Errors { get; set; } = null!;
        public virtual DbSet<Log> Logs { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<Zakazka> Zakazkas { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=HRABCAK;Database=TOYOTA_DB2;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Dokument>(entity =>
            {
                entity.ToTable("Dokument");

                entity.Property(e => e.DokumentId).HasColumnName("DokumentID");

                entity.Property(e => e.DokumentPlatny)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.NazovDokumentu).HasMaxLength(64);

                entity.Property(e => e.NazovSuboru).HasMaxLength(64);

                entity.Property(e => e.Poznamka)
                    .HasMaxLength(128)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Skupina).HasDefaultValueSql("((-1))");

                entity.Property(e => e.Vytvorene)
                    .HasPrecision(0)
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Vytvoril)
                    .HasMaxLength(32)
                    .HasDefaultValueSql("(user_name())");

                entity.Property(e => e.ZakazkaTg)
                    .HasMaxLength(16)
                    .HasColumnName("ZakazkaTG")
                    .IsFixedLength();

                entity.Property(e => e.Zmenene).HasPrecision(0);

                entity.Property(e => e.Zmenil)
                    .HasMaxLength(32)
                    .HasDefaultValueSql("('')");

                entity.HasOne(d => d.ZakazkaTgNavigation)
                    .WithMany(p => p.Dokuments)
                    .HasForeignKey(d => d.ZakazkaTg)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Dokument_Zakazka");
            });

            modelBuilder.Entity<DokumentDetail>(entity =>
            {
                entity.HasKey(e => e.DetailId);

                entity.ToTable("Dokument_Detail");

                entity.Property(e => e.DetailId)
                    .HasColumnName("DetailID")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.DokumentId).HasColumnName("DokumentID");

                entity.Property(e => e.NazovDokumentu).HasMaxLength(32);

                entity.Property(e => e.Platny)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('A')")
                    .IsFixedLength();

                entity.Property(e => e.Poznamka)
                    .HasMaxLength(128)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Vytvorene)
                    .HasPrecision(0)
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Vytvoril)
                    .HasMaxLength(32)
                    .HasDefaultValueSql("(user_name())");

                entity.Property(e => e.Zmenene).HasPrecision(0);

                entity.Property(e => e.Zmenil)
                    .HasMaxLength(32)
                    .HasDefaultValueSql("('')");

                entity.HasOne(d => d.Dokument)
                    .WithMany(p => p.DokumentDetails)
                    .HasForeignKey(d => d.DokumentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Dokument_Detail_Dokument");
            });

            modelBuilder.Entity<Error>(entity =>
            {
                entity.HasKey(e => e.ErrorLogId);

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
                entity.Property(e => e.LogId).HasColumnName("LogID");

                entity.Property(e => e.LogDate)
                    .HasPrecision(0)
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.LogMessage).HasMaxLength(128);

                entity.Property(e => e.TableName)
                    .HasMaxLength(64)
                    .IsFixedLength();

                entity.Property(e => e.UserAction)
                    .HasMaxLength(20)
                    .HasDefaultValueSql("(' ')")
                    .IsFixedLength();

                entity.Property(e => e.UserName)
                    .HasMaxLength(32)
                    .HasDefaultValueSql("(user_name())");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(e => e.RoleId)
                    .ValueGeneratedNever()
                    .HasColumnName("RoleID");

                entity.Property(e => e.Aktivny)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('A')")
                    .IsFixedLength();

                entity.Property(e => e.Poznamka)
                    .HasMaxLength(64)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.RoleName)
                    .HasMaxLength(24)
                    .IsFixedLength();

                entity.Property(e => e.Vytvorene)
                    .HasPrecision(0)
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Vytvoril)
                    .HasMaxLength(32)
                    .HasDefaultValueSql("(user_name())");

                entity.Property(e => e.Zmenene).HasPrecision(0);

                entity.Property(e => e.Zmenil)
                    .HasMaxLength(32)
                    .HasDefaultValueSql("('')");
            });

            modelBuilder.Entity<Zakazka>(entity =>
            {
                entity.HasKey(e => e.ZakazkaTg)
                    .HasName("PK_Zakazka1");

                entity.ToTable("Zakazka");

                entity.Property(e => e.ZakazkaTg)
                    .HasMaxLength(16)
                    .HasColumnName("ZakazkaTG")
                    .IsFixedLength();

                entity.Property(e => e.CisloProtokolu)
                    .HasMaxLength(16)
                    .IsFixedLength();

                entity.Property(e => e.Cws)
                    .HasMaxLength(32)
                    .HasColumnName("CWS")
                    .IsFixedLength();

                entity.Property(e => e.Platna)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('A')")
                    .IsFixedLength();

                entity.Property(e => e.Poznamka).HasMaxLength(128);

                entity.Property(e => e.Ukoncena)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('N')")
                    .IsFixedLength();

                entity.Property(e => e.Vin)
                    .HasMaxLength(34)
                    .HasColumnName("VIN")
                    .IsFixedLength();

                entity.Property(e => e.Vytvorene)
                    .HasPrecision(0)
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Vytvoril)
                    .HasMaxLength(32)
                    .HasDefaultValueSql("(user_name())");

                entity.Property(e => e.ZakazkaId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ZakazkaID");

                entity.Property(e => e.ZakazkaTb)
                    .HasMaxLength(16)
                    .HasColumnName("ZakazkaTB")
                    .IsFixedLength();

                entity.Property(e => e.Zmenene)
                    .HasPrecision(0)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Zmenil)
                    .HasMaxLength(32)
                    .HasDefaultValueSql("(' ')");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
