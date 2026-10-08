using Api.DTOs.Negocio;
using Api.Models.Entities;
using Api.Repositories.Interfaces;
using Api.Services.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Services.Implementations
{
    public class NegocioService : INegocioService
    {
        private readonly INegocioRepository _negocioRepository;
        private readonly IUnitOfWork _unitOfWork;

        public NegocioService(INegocioRepository negocioRepository, IUnitOfWork unitOfWork)
        {
            _negocioRepository = negocioRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<NegocioResponseDto?> GetAsync(CancellationToken cancellationToken = default)
        {
            var negocio = await _negocioRepository.GetAsync(cancellationToken);
            return negocio != null ? MapToDTO(negocio) : null;
        }

        public async Task<NegocioResponseDto> CrearAsync(NegocioCrearRequest dto, CancellationToken cancellationToken = default)
        {
            var existe = await _negocioRepository.ExisteAsync(cancellationToken);
            if (existe)
            {
                throw new InvalidOperationException("Ya existe un negocio configurado. Use la actualización en su lugar.");
            }

            var negocio = new Negocio
            {
                Nombre = dto.Nombre,
                RazonSocial = dto.RazonSocial,
                IdentificacionFiscal = dto.IdentificacionFiscal,
                Direccion = dto.Direccion,
                Telefono = dto.Telefono,
                Email = dto.Email,
                MensajePieComprobante = dto.MensajePieComprobante,
                Logo = dto.Logo,
                PreferenciasPantalla = dto.PreferenciasPantalla,
                ConfiguracionImpresion = dto.ConfiguracionImpresion,
                MonedaDefecto = dto.MonedaDefecto,
                SimboloMoneda = dto.SimboloMoneda,
                ZonaHoraria = dto.ZonaHoraria,
                Activo = dto.Activo,
                FechaCreacion = DateTime.UtcNow,
                FechaActualizacion = DateTime.UtcNow
            };

            await _negocioRepository.AddAsync(negocio, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return MapToDTO(negocio);
        }

        public async Task<NegocioResponseDto?> ActualizarAsync(NegocioActualizarRequest dto, CancellationToken cancellationToken = default)
        {
            var existente = await _negocioRepository.GetByIdAsync(dto.Id, cancellationToken);
            if (existente == null) return null;

            existente.Nombre = dto.Nombre;
            existente.RazonSocial = dto.RazonSocial;
            existente.IdentificacionFiscal = dto.IdentificacionFiscal;
            existente.Direccion = dto.Direccion;
            existente.Telefono = dto.Telefono;
            existente.Email = dto.Email;
            existente.MensajePieComprobante = dto.MensajePieComprobante;
            existente.Logo = dto.Logo;
            existente.PreferenciasPantalla = dto.PreferenciasPantalla;
            existente.ConfiguracionImpresion = dto.ConfiguracionImpresion;
            existente.MonedaDefecto = dto.MonedaDefecto;
            existente.SimboloMoneda = dto.SimboloMoneda;
            existente.ZonaHoraria = dto.ZonaHoraria;
            existente.Activo = dto.Activo;
            existente.FechaActualizacion = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return MapToDTO(existente);
        }

        public async Task<bool> ExisteAsync(CancellationToken cancellationToken = default)
        {
            return await _negocioRepository.ExisteAsync(cancellationToken);
        }

        private static NegocioResponseDto MapToDTO(Negocio negocio)
        {
            return new NegocioResponseDto
            {
                Id = negocio.Id,
                Nombre = negocio.Nombre,
                RazonSocial = negocio.RazonSocial,
                IdentificacionFiscal = negocio.IdentificacionFiscal,
                Direccion = negocio.Direccion,
                Telefono = negocio.Telefono,
                Email = negocio.Email,
                MensajePieComprobante = negocio.MensajePieComprobante,
                Logo = negocio.Logo,
                PreferenciasPantalla = negocio.PreferenciasPantalla,
                ConfiguracionImpresion = negocio.ConfiguracionImpresion,
                MonedaDefecto = negocio.MonedaDefecto,
                SimboloMoneda = negocio.SimboloMoneda,
                ZonaHoraria = negocio.ZonaHoraria,
                Activo = negocio.Activo,
                FechaCreacion = negocio.FechaCreacion,
                FechaActualizacion = negocio.FechaActualizacion
            };
        }
    }
}