using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace PA.TOYOTA.DB
{
    //MH: 21.04.2022
    public partial class ToyotaContext : DbContext
    {

        public string? ConnectionString { get; set; }
        public ToyotaContext()
        {
            ConnectionString = Database.GetConnectionString();
        }

        public ToyotaContext(DbContextOptions<ToyotaContext> options)
            : base(options)
        {
            ConnectionString = Database.GetConnectionString();
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
                optionsBuilder.UseSqlServer("Server=HRABCAK;Database=TOYOTA_DB;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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

                entity.Property(e => e.Vytvorene).HasPrecision(0);

                entity.Property(e => e.Vytvoril).HasMaxLength(32);

                entity.Property(e => e.ZakazkaTg)
                    .HasMaxLength(12)
                    .HasColumnName("ZakazkaTG")
                    .IsFixedLength();

                entity.Property(e => e.Zmenene).HasPrecision(0);

                entity.Property(e => e.Zmenil).HasMaxLength(32);

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

                entity.Property(e => e.Vytvorene).HasPrecision(0);

                entity.Property(e => e.Vytvoril).HasMaxLength(32);

                entity.Property(e => e.Zmenene).HasPrecision(0);

                entity.Property(e => e.Zmenil).HasMaxLength(32);

                entity.HasOne(d => d.Dokument)
                    .WithMany(p => p.DokumentDetails)
                    .HasForeignKey(d => d.DokumentId)
                    .HasConstraintName("FK_Dokument_Detail_Dokument");
            });

            modelBuilder.Entity<Error>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Error");

                entity.Property(e => e.ErrorDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ErrorLogId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ErrorLogID");

                entity.Property(e => e.ErrorMsg)
                    .HasMaxLength(256)
                    .IsUnicode(false);

                entity.Property(e => e.ErrorProcedure)
                    .HasMaxLength(256)
                    .IsUnicode(false);

                entity.Property(e => e.User)
                    .HasMaxLength(64)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(user_name())");
            });

            modelBuilder.Entity<Log>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Log");

                entity.Property(e => e.LogDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.LogId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("LogID");

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
                entity.HasNoKey();

                entity.ToTable("Role");

                entity.Property(e => e.Aktivny)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('A')")
                    .IsFixedLength();

                entity.Property(e => e.Poznamka)
                    .HasMaxLength(64)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.RoleId).HasColumnName("RoleID");

                entity.Property(e => e.RoleName)
                    .HasMaxLength(24)
                    .IsFixedLength();

                entity.Property(e => e.Vytvorene)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Vytvoril)
                    .HasMaxLength(32)
                    .HasDefaultValueSql("(user_name())")
                    .IsFixedLength();

                entity.Property(e => e.Zmenene).HasColumnType("datetime");

                entity.Property(e => e.Zmenil)
                    .HasMaxLength(32)
                    .HasDefaultValueSql("('')")
                    .IsFixedLength();
            });

            modelBuilder.Entity<Zakazka>(entity =>
            {
                entity.HasKey(e => e.ZakazkaTg);

                entity.ToTable("Zakazka");

                entity.Property(e => e.ZakazkaTg)
                    .HasMaxLength(12)
                    .HasColumnName("ZakazkaTG")
                    .IsFixedLength();

                entity.Property(e => e.CisloProtokolu)
                    .HasMaxLength(16)
                    .IsFixedLength();

                entity.Property(e => e.Cws)
                    .HasMaxLength(32)
                    .HasColumnName("CWS")
                    .IsFixedLength();

                entity.Property(e => e.Poznamka).HasMaxLength(128);

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
