using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Api.Models.Entities;

namespace Api.Data.Configurations
{
    public class ProveedorConfiguration : IEntityTypeConfiguration<Proveedor>
    {
        public void Configure(EntityTypeBuilder<Proveedor> builder)
        {
            builder.ToTable("Proveedores");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Nombre)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.Telefono)
                .HasMaxLength(20)
                .IsUnicode(false);

            builder.Property(p => p.RTN)
                .HasMaxLength(25)
                .IsUnicode(false);

            // Relationship to Compras
            builder.HasMany(p => p.Compras)
                .WithOne(c => c.Proveedor)
                .HasForeignKey(c => c.ProveedorId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}