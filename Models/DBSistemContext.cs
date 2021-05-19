using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace AplicatieCamine.Models
{
    public partial class DBSistemContext : DbContext
    {
        public DBSistemContext()
        {
        }

        public DBSistemContext(DbContextOptions<DBSistemContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Administratori> Administratori { get; set; }
        public virtual DbSet<Camere> Camere { get; set; }
        public virtual DbSet<Camine> Camine { get; set; }
        public virtual DbSet<Student> Student { get; set; }
        public virtual DbSet<Tichet> Tichet { get; set; }
        public virtual DbSet<Applicant> Applicant { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=tcp:server-db-camine.database.windows.net,1433;Initial Catalog=DBSistem;Persist Security Info=False;User ID=adminUser;Password=admincamine123!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Administratori>(entity =>
            {
                entity.HasKey(e => e.IdAdmin)
                    .HasName("PK__ADMINIST__89472E9570B976EA");

                entity.ToTable("ADMINISTRATORI");

                entity.Property(e => e.IdAdmin)
                    .HasColumnName("id_admin")
                    .ValueGeneratedNever();

                entity.Property(e => e.Adresa)
                    .IsRequired()
                    .HasColumnName("adresa")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasColumnName("email")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.IdCamin).HasColumnName("id_camin");

                entity.Property(e => e.NrTelefon)
                    .IsRequired()
                    .HasColumnName("nr_telefon")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Nume)
                    .IsRequired()
                    .HasColumnName("nume")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdCaminNavigation)
                    .WithMany(p => p.Administratori)
                    .HasForeignKey(d => d.IdCamin)
                    .HasConstraintName("FK__ADMINISTR__id_ca__76969D2E");
            });

            modelBuilder.Entity<Camere>(entity =>
            {
                entity.HasKey(e => e.IdCamera)
                    .HasName("PK__CAMERE__CA0FE41D09B2B6E0");

                entity.ToTable("CAMERE");

                entity.Property(e => e.IdCamera)
                    .HasColumnName("id_camera")
                    .ValueGeneratedNever();

                entity.Property(e => e.Descriere)
                    .IsRequired()
                    .HasColumnName("descriere")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.IdCamin).HasColumnName("id_camin");

                entity.Property(e => e.LimitaNrStudenti).HasColumnName("limita_nr_studenti");

                entity.Property(e => e.NrStudentiCazati).HasColumnName("nr_studenti_cazati");

                entity.Property(e => e.NrCamera)
                    .IsRequired()
                    .HasColumnName("nr_camera")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdCaminNavigation)
                    .WithMany(p => p.Camere)
                    .HasForeignKey(d => d.IdCamin)
                    .HasConstraintName("FK__CAMERE__id_camin__70DDC3D8");
            });

            modelBuilder.Entity<Camine>(entity =>
            {
                entity.HasKey(e => e.IdCamin)
                    .HasName("PK__CAMINE__C1F3C2E80E4259B9");

                entity.ToTable("CAMINE");

                entity.Property(e => e.IdCamin)
                    .HasColumnName("id_camin")
                    .ValueGeneratedNever();

                entity.Property(e => e.Adresa)
                    .IsRequired()
                    .HasColumnName("adresa")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.Facultate)
                    .IsRequired()
                    .HasColumnName("facultate")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.NrCamere).HasColumnName("nr_camere");

                entity.Property(e => e.NrLocuri).HasColumnName("nr_locuri");

            });

            modelBuilder.Entity<Applicant>(entity =>
            {
                entity.HasKey(e => e.IdApplicant)
                    .HasName("PK__APPLICAN__67EE270EA55EE147");

                entity.ToTable("APPLICANT");

                entity.Property(e => e.IdApplicant)
                    .ValueGeneratedNever()
                    .HasColumnName("id_applicant");

                entity.Property(e => e.Adresa)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("adresa");

                entity.Property(e => e.An).HasColumnName("an");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("email");

                entity.Property(e => e.Facultate)
                    .IsRequired()
                    .HasMaxLength(40)
                    .IsUnicode(false)
                    .HasColumnName("facultate");

                entity.Property(e => e.Nume)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("nume");

                entity.Property(e => e.Prenume)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("prenume");

                entity.Property(e => e.Varsta).HasColumnName("varsta");
            });


            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(e => e.IdStudent)
                    .HasName("PK__STUDENT__2BE2EBB6FE8FEE61");

                entity.ToTable("STUDENT");

                entity.Property(e => e.IdStudent)
                    .HasColumnName("id_student")
                    .ValueGeneratedNever();

                entity.Property(e => e.Adresa)
                    .IsRequired()
                    .HasColumnName("adresa")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.An).HasColumnName("an");

                entity.Property(e => e.DataCazare)
                    .HasColumnName("data_cazare")
                    .HasColumnType("date");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasColumnName("email")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Facultate)
                    .IsRequired()
                    .HasColumnName("facultate")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.IdCamera).HasColumnName("id_camera");

                entity.Property(e => e.Nume)
                    .IsRequired()
                    .HasColumnName("nume")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Prenume)
                    .IsRequired()
                    .HasColumnName("prenume")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Varsta).HasColumnName("varsta");

                entity.HasOne(d => d.IdCameraNavigation)
                    .WithMany(p => p.Student)
                    .HasForeignKey(d => d.IdCamera)
                    .HasConstraintName("FK__STUDENT__id_came__73BA3083");
            });

            modelBuilder.Entity<Tichet>(entity =>
            {
                entity.HasKey(e => e.IdTichet)
                    .HasName("PK__TICHET__4E9E5044B704390B");

                entity.ToTable("TICHET");

                entity.Property(e => e.IdTichet)
                    .HasColumnName("id_tichet")
                    .ValueGeneratedNever();

                entity.Property(e => e.DataEmitere)
                    .HasColumnName("data_emitere")
                    .HasColumnType("date");

                entity.Property(e => e.DateRezolvare)
                    .HasColumnName("date_rezolvare")
                    .HasColumnType("date");

                entity.Property(e => e.Detalii)
                    .IsRequired()
                    .HasColumnName("detalii")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.IdStudent).HasColumnName("id_student");

                entity.Property(e => e.StatusTichet).HasColumnName("status_tichet");

                entity.Property(e => e.TipTichet).HasColumnName("tip_tichet");

                entity.Property(e => e.IdCamera).HasColumnName("id_camera");

                entity.Property(e => e.FileName)
                .HasColumnName("file_name")
                .IsUnicode(false);

                entity.HasOne(d => d.IdStudentNavigation)
                    .WithMany(p => p.Tichet)
                    .HasForeignKey(d => d.IdStudent)
                    .HasConstraintName("FK__TICHET__id_stude__7C4F7684");

                //entity.HasOne(d => d.IdCameraNavigation).WithMany(p => p.).HasForeignKey(d => d.IdCamera).HasConstraintName("camera_constr");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
