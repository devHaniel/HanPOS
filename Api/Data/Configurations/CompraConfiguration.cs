using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Api.Models.Entities;

namespace Api.Data.Configurations
{
    public class CompraConfiguration : IEntityTypeConfiguration<Compra>
    {
        public void Configure(EntityTypeBuilder<Compra> builder)
        {
            builder.ToTable("Compras");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Fecha)
                .IsRequired();

            builder.Property(c => c.Subtotal)
                .HasColumnType("decimal(18,2)");

            builder.Property(c => c.Impuesto)
                .HasColumnType("decimal(18,2)");

            builder.Property(c => c.Total)
                .HasColumnType("decimal(18,2)");

            builder.Property(c => c.Estado)
                .IsRequired();

            // Relationship to Usuario
            builder.HasOne(c => c.Usuario)
                .WithMany(u => u.Compras)
                .HasForeignKey(c => c.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relationship to Proveedor
            builder.HasOne(c => c.Proveedor)
                .WithMany(p => p.Compras)
                .HasForeignKey(c => c.ProveedorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relationship to DetallesCompra
            builder.HasMany(c => c.Detalles)
                .WithOne(d => d.Compra)
                .HasForeignKey(d => d.CompraId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}