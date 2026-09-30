using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Api.Models.Entities;

namespace Api.Data.Configurations
{
    public class MovimientoCajaConfiguration : IEntityTypeConfiguration<MovimientoCaja>
    {
        public void Configure(EntityTypeBuilder<MovimientoCaja> builder)
        {
            builder.ToTable("MovimientosCaja");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.Monto)
                .HasColumnType("decimal(18,2)");

            builder.Property(m => m.Concepto)
                .HasMaxLength(200);

            // Relationship to Caja
            builder.HasOne(m => m.Caja)
                .WithMany(c => c.Movimientos)
                .HasForeignKey(m => m.CajaId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}