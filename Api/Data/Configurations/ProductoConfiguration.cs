using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Api.Models.Entities;

namespace Api.Data.Configurations
{
    public class ProductoConfiguration : IEntityTypeConfiguration<Producto>
    {
        public void Configure(EntityTypeBuilder<Producto> builder)
        {
            builder.ToTable("Productos");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Codigo)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(p => p.Nombre)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.PrecioVenta)
                .HasColumnType("decimal(18,2)");

            builder.Property(p => p.PrecioCompra)
                .HasColumnType("decimal(18,2)");

            builder.Property(p => p.Stock)
                .HasColumnType("decimal(18,2)");

            builder.Property(p => p.StockMinimo)
                .HasColumnType("decimal(18,2)");

            builder.Property(p => p.Activo)
                .IsRequired();

            // Index on Codigo (unique)
            builder.HasIndex(p => p.Codigo)
                .IsUnique();

            // Relationship to Categoria
            builder.HasOne(p => p.Categoria)
                .WithMany(c => c.Productos)
                .HasForeignKey(p => p.CategoriaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relationships to DetalleVenta and DetalleCompra
            builder.HasMany(p => p.DetallesVenta)
                .WithOne(d => d.Producto)
                .HasForeignKey(d => d.ProductoId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.DetallesCompra)
                .WithOne(d => d.Producto)
                .HasForeignKey(d => d.ProductoId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}