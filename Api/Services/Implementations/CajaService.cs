using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Api.DTOs.Caja;
using Api.DTOs.MovimientoCaja;
using Api.DTOs.Paginacion;
using Api.Models.Entities;
using Api.Repositories.Interfaces;
using Api.Services.Interfaces;

namespace Api.Services.Implementations
{
    public class CajaService : ICajaService
    {
        private readonly ICajaRepository _repository;
        private readonly IMovimientoCajaRepository _movimientoRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CajaService(ICajaRepository repository, IMovimientoCajaRepository movimientoRepository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _movimientoRepository = movimientoRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IReadOnlyList<CajaResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var cajas = await _repository.GetAllAsync(cancellationToken);
            return cajas.Select(MapToDTO).ToList().AsReadOnly();
        }

        public async Task<PagedResult<CajaResponseDto>> GetAllPagedAsync(int pagina = 1, int cantidad = 10, CancellationToken cancellationToken = default)
        {
            var (items, total) = await _repository.GetAllPagedAsync(pagina, cantidad, cancellationToken);
            return new PagedResult<CajaResponseDto>
            {
                Items = items.Select(MapToDTO).ToList(),
                Pagina = pagina,
                Cantidad = cantidad,
                Total = total
            };
        }

        public async Task<CajaResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var caja = await _repository.GetByIdAsync(id, cancellationToken);
            return caja != null ? MapToDTO(caja) : null;
        }

        public async Task<CajaResponseDto?> GetAbiertaAsync(CancellationToken cancellationToken = default)
        {
            var caja = await _repository.GetAbiertaAsync(cancellationToken);
            return caja != null ? MapToDTO(caja) : null;
        }

        public async Task<CajaResponseDto> CrearAsync(CajaCrearRequest dto, CancellationToken cancellationToken = default)
        {
            // Validar que no exista una caja abierta
            var cajaAbierta = await _repository.GetAbiertaAsync(cancellationToken);
            if (cajaAbierta != null)
            {
                throw new InvalidOperationException("Ya existe una caja abierta. Debe cerrarla antes de abrir una nueva.");
            }
            
            var caja = new Caja
            {
                FechaApertura = dto.FechaApertura,
                MontoInicial = dto.MontoInicial,
                FechaCierre = null,
                UsuarioId = dto.UsuarioId
            };

            await _repository.AddAsync(caja, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return MapToDTO(caja);
        }

        public async Task<CajaResponseDto?> CerrarAsync(int id, CancellationToken cancellationToken = default)
        {
            var caja = await _repository.GetAbiertaAsync(cancellationToken);
            if (caja == null || caja.Id != id) return null;

            caja.FechaCierre = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return MapToDTO(caja);
        }

        public async Task<CajaResponseDto?> ActualizarAsync(CajaActualizarRequest dto, CancellationToken cancellationToken = default)
        {
            var existente = await _repository.GetByIdAsync(dto.Id, cancellationToken);
            if (existente == null) return null;

            existente.FechaApertura = dto.FechaApertura;
            existente.FechaCierre = dto.FechaCierre;
            existente.MontoInicial = dto.MontoInicial;
            existente.MontoFinal = dto.MontoFinal;
            existente.UsuarioId = dto.UsuarioId;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return MapToDTO(existente);
        }

        public async Task<bool> EliminarAsync(int id, CancellationToken cancellationToken = default)
        {
            var caja = await _repository.GetByIdAsync(id, cancellationToken);
            if (caja == null) return false;

            await _repository.RemoveAsync(caja, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<IReadOnlyList<MovimientoCajaResponseDto>> GetMovimientosAsync(int cajaId, CancellationToken cancellationToken = default)
        {
            var movimientos = await _movimientoRepository.GetPorCajaAsync(cajaId, cancellationToken);
            return movimientos.Select(MapToMovimientoDTO).ToList().AsReadOnly();
        }

        private static CajaResponseDto MapToDTO(Caja caja)
        {
            return new CajaResponseDto
            {
                Id = caja.Id,
                FechaApertura = caja.FechaApertura,
                FechaCierre = caja.FechaCierre,
                MontoInicial = caja.MontoInicial,
                MontoFinal = caja.MontoFinal,
                UsuarioId = caja.UsuarioId
            };
        }

        private static MovimientoCajaResponseDto MapToMovimientoDTO(MovimientoCaja movimiento)
        {
            return new MovimientoCajaResponseDto
            {
                Id = movimiento.Id,
                Monto = movimiento.Monto,
                TipoMovimiento = (int)movimiento.Tipo,
                Concepto = movimiento.Concepto,
                Fecha = movimiento.Fecha,
                CajaId = movimiento.CajaId
            };
        }
    }
}