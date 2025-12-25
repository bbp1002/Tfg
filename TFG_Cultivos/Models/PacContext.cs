using Microsoft.EntityFrameworkCore;
using TFG_Cultivos.Models;

namespace TFG_Cultivos.Models
{
    public class PacContext :DbContext
    {
        public PacContext(DbContextOptions<PacContext> options)
            : base(options)
        {
        }
        public DbSet<Parcelas> Parcelas { get; set; }
        public DbSet<Recintos> Recintos { get; set; }
        public DbSet<DatoAgronomico> DatoAgronomico { get; set; }


        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Campania>().ToTable("campanias");
        //    modelBuilder.Entity<Provincia>().ToTable("provincias");
        //    modelBuilder.Entity<Municipio>().ToTable("municipios");
        //    modelBuilder.Entity<Recintos>().ToTable("recintos_sigpac");
        //    modelBuilder.Entity<DatoAgronomico>().ToTable("datos_agronomicos");
            
        //    modelBuilder.Entity<Campania>().HasKey(c => c.IdCampania);
        //    modelBuilder.Entity<Provincia>().HasKey(p => p.IdProvincia);
        //    modelBuilder.Entity<Municipio>().HasKey(m => m.IdMunicipio);
        //    modelBuilder.Entity<Recintos>().HasKey(r => r.IdRecinto);
        //    modelBuilder.Entity<DatoAgronomico>().HasKey(d => d.IdDato);

        //    modelBuilder.Entity<Municipio>()
        //        .HasOne(m => m.Provincia)
        //        .WithMany(p => p.Municipios)
        //        .HasForeignKey(m => m.IdProvincia);

        //    modelBuilder.Entity<Recintos>()
        //        .HasOne(r => r.Municipio)
        //        .WithMany(m => m.Recintos)
        //        .HasForeignKey(r => r.IdMunicipio);

        //    modelBuilder.Entity<DatoAgronomico>()
        //        .HasOne(d => d.Recinto)
        //        .WithMany(r => r.DatosAgronomicos)
        //        .HasForeignKey(d => d.IdRecinto);

        //    modelBuilder.Entity<DatoAgronomico>()
        //        .HasOne(d => d.Campania)
        //        .WithMany(c => c.DatosAgronomicos)
        //        .HasForeignKey(d => d.IdCampania);

        //    modelBuilder.Entity<Parcelas>(entity =>
        //    {
        //        entity.ToTable("parcelas", "public");

        //        entity.HasKey(e => e.Id);
        //        entity.Property(e => e.Id).HasColumnName("id");
        //        entity.Property(e => e.Provincia).HasColumnName("provincia");
        //        entity.Property(e => e.TerminoMunicipal).HasColumnName("municipio");
        //        entity.Property(e => e.Poligono).HasColumnName("poligono");
        //        entity.Property(e => e.ParcelaNumero).HasColumnName("parcela");
        //        entity.Property(e => e.Recinto).HasColumnName("recinto");
        //        entity.Property(e => e.Superficie).HasColumnName("superficie");
        //        entity.Property(e => e.SuperficieCultivada).HasColumnName("cultivo");
        //        //entity.Property(e => e.NombrePersonalizado).HasColumnName("nombre_personalizado");
        //        //entity.Property(e => e.Ano).HasColumnName("ano");
        //    });
        //}

    }
}