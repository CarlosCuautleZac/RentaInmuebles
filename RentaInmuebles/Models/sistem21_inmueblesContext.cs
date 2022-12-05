using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace RentaInmuebles.Models
{
    public partial class sistem21_inmueblesContext : DbContext
    {
        public sistem21_inmueblesContext()
        {
        }

        public sistem21_inmueblesContext(DbContextOptions<sistem21_inmueblesContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Ciudad> Ciudad { get; set; } = null!;
        public virtual DbSet<Propiedad> Propiedad { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("utf8_general_ci")
                .HasCharSet("utf8");

            modelBuilder.Entity<Ciudad>(entity =>
            {
                entity.ToTable("ciudad");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.Estado).HasMaxLength(100);

                entity.Property(e => e.Nombre).HasMaxLength(100);

                entity.Property(e => e.Pais).HasMaxLength(100);
            });

            modelBuilder.Entity<Propiedad>(entity =>
            {
                entity.ToTable("propiedad");

                entity.HasIndex(e => e.Idciudad, "fkPropiedadCiudad_idx");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.CantBaños)
                    .HasColumnType("int(11)")
                    .HasColumnName("cant_baños");

                entity.Property(e => e.CantCochera)
                    .HasColumnType("int(11)")
                    .HasColumnName("cant_cochera");

                entity.Property(e => e.CantCuartos)
                    .HasColumnType("int(11)")
                    .HasColumnName("cant_cuartos");

                entity.Property(e => e.Descripcion)
                    .HasColumnType("text")
                    .HasColumnName("descripcion");

                entity.Property(e => e.Direccion)
                    .HasMaxLength(200)
                    .HasColumnName("direccion");

                entity.Property(e => e.Disponible)
                    .HasColumnType("int(11)")
                    .HasColumnName("disponible");

                entity.Property(e => e.Idciudad)
                    .HasColumnType("int(11)")
                    .HasColumnName("idciudad");

                entity.Property(e => e.Nombre)
                    .HasMaxLength(30)
                    .HasColumnName("nombre");

                entity.Property(e => e.Precio)
                    .HasPrecision(10, 2)
                    .HasColumnName("precio");

                entity.HasOne(d => d.IdciudadNavigation)
                    .WithMany(p => p.Propiedad)
                    .HasForeignKey(d => d.Idciudad)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fkPropiedadCiudad");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
