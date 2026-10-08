using Api.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Data.Configurations
{
    public class NegocioConfiguration : IEntityTypeConfiguration<Negocio>
    {
        public void Configure(EntityTypeBuilder<Negocio> builder)
        {
            builder.ToTable("Negocios");

            builder.HasKey(n => n.Id);

            builder.Property(n => n.Nombre)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(n => n.RazonSocial)
                .HasMaxLength(200);

            builder.Property(n => n.IdentificacionFiscal)
                .HasMaxLength(20);

            builder.Property(n => n.Direccion)
                .HasMaxLength(300);

            builder.Property(n => n.Telefono)
                .HasMaxLength(30);

            builder.Property(n => n.Email)
                .HasMaxLength(150);

            builder.Property(n => n.MensajePieComprobante)
                .HasMaxLength(500);

            builder.Property(n => n.Logo)
                .HasMaxLength(5000); // Base64 puede ser largo

            builder.Property(n => n.PreferenciasPantalla)
                .HasColumnType("jsonb");

            builder.Property(n => n.ConfiguracionImpresion)
                .HasColumnType("jsonb");

            builder.Property(n => n.MonedaDefecto)
                .IsRequired()
                .HasMaxLength(3)
                .HasDefaultValue("PEN");

            builder.Property(n => n.SimboloMoneda)
                .IsRequired()
                .HasMaxLength(5)
                .HasDefaultValue("S/");

            builder.Property(n => n.ZonaHoraria)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValue("America/Lima");

            builder.Property(n => n.Activo)
                .HasDefaultValue(true);

            builder.Property(n => n.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(n => n.FechaActualizacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Índice para asegurar singleton (opcional, pero bueno tenerlo)
            builder.HasIndex(n => n.Id)
                .IsUnique();
        }
    }
}