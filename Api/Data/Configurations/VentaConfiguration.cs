using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Api.Models.Entities;

namespace Api.Data.Configurations
{
    public class VentaConfiguration : IEntityTypeConfiguration<Venta>
    {
        public void Configure(EntityTypeBuilder<Venta> builder)
        {
            builder.ToTable("Ventas");

            builder.HasKey(v => v.Id);

            builder.Property(v => v.Fecha)
                .IsRequired();

            builder.Property(v => v.Subtotal)
                .HasColumnType("decimal(18,2)");

            builder.Property(v => v.Impuesto)
                .HasColumnType("decimal(18,2)");

            builder.Property(v => v.Total)
                .HasColumnType("decimal(18,2)");

            builder.Property(v => v.MetodoPago)
                .IsRequired();

            builder.Property(v => v.Estado)
                .IsRequired();

            // Relationship to Usuario
            builder.HasOne(v => v.Usuario)
                .WithMany(u => u.Ventas)
                .HasForeignKey(v => v.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relationship to Cliente (optional)
            builder.HasOne(v => v.Cliente)
                .WithMany(c => c.Ventas)
                .HasForeignKey(v => v.ClienteId)
                .OnDelete(DeleteBehavior.SetNull);

            // Relationship to Detalles
            builder.HasMany(v => v.Detalles)
                .WithOne(d => d.Venta)
                .HasForeignKey(d => d.VentaId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}