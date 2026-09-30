using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Api.Models.Entities;

namespace Api.Data.Configurations
{
    public class CajaConfiguration : IEntityTypeConfiguration<Caja>
    {
        public void Configure(EntityTypeBuilder<Caja> builder)
        {
            builder.ToTable("Cajas");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.FechaApertura)
                .IsRequired();

            builder.Property(c => c.FechaCierre)
                .IsRequired(false);

            builder.Property(c => c.MontoInicial)
                .HasColumnType("decimal(18,2)");

            builder.Property(c => c.MontoFinal)
                .HasColumnType("decimal(18,2)");

            // Relationship to Usuario
            builder.HasOne(c => c.Usuario)
                .WithMany(u => u.Cajas)
                .HasForeignKey(c => c.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relationship to MovimientosCaja
            builder.HasMany(c => c.Movimientos)
                .WithOne(m => m.Caja)
                .HasForeignKey(m => m.CajaId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}