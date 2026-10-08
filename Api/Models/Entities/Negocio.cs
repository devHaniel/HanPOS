using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Models.Entities
{
    /// <summary>
    /// Entidad que representa el negocio/empresa del sistema.
    /// Solo debe existir UN registro (singleton) que representa la configuración global.
    /// </summary>
    public class Negocio : IEntity
    {
        public int Id { get; set; }

        /// <summary>
        /// Nombre comercial del negocio
        /// </summary>
        public string Nombre { get; set; } = null!;

        /// <summary>
        /// Razón social (nombre legal)
        /// </summary>
        public string? RazonSocial { get; set; }

        /// <summary>
        /// RUC / NIT / Identificación fiscal
        /// </summary>
        public string? IdentificacionFiscal { get; set; }

        /// <summary>
        /// Dirección fiscal
        /// </summary>
        public string? Direccion { get; set; }

        /// <summary>
        /// Teléfono de contacto
        /// </summary>
        public string? Telefono { get; set; }

        /// <summary>
        /// Email de contacto
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Mensaje que aparece al pie de los comprobantes (tickets, facturas, etc.)
        /// </summary>
        public string? MensajePieComprobante { get; set; }

        /// <summary>
        /// Logo del negocio (Base64 o URL)
        /// </summary>
        public string? Logo { get; set; }

        /// <summary>
        /// Preferencias de pantalla en JSON (tema, modo compacto, etc.)
        /// </summary>
        public string? PreferenciasPantalla { get; set; }

        /// <summary>
        /// Configuración de impresión en JSON (tamaño papel, márgenes, etc.)
        /// </summary>
        public string? ConfiguracionImpresion { get; set; }

        /// <summary>
        /// Moneda por defecto (ISO 4217, ej: PEN, USD, EUR)
        /// </summary>
        public string MonedaDefecto { get; set; } = "PEN";

        /// <summary>
        /// Símbolo de la moneda
        /// </summary>
        public string SimboloMoneda { get; set; } = "S/";

        /// <summary>
        /// Zona horaria (ej: America/Lima)
        /// </summary>
        public string ZonaHoraria { get; set; } = "America/Lima";

        /// <summary>
        /// Indica si el negocio está activo
        /// </summary>
        public bool Activo { get; set; } = true;

        /// <summary>
        /// Fecha de creación del registro
        /// </summary>
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Fecha de última actualización
        /// </summary>
        public DateTime FechaActualizacion { get; set; } = DateTime.UtcNow;
    }
}