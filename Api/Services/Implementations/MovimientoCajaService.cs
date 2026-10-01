using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Api.DTOs.MovimientoCaja;
using Api.Models.Entities;
using Api.Repositories.Interfaces;
using Api.Services.Interfaces;

namespace Api.Services.Implementations
{
    public class MovimientoCajaService : IMovimientoCajaService
    {
        private readonly IMovimientoCajaRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public MovimientoCajaService(IMovimientoCajaRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IReadOnlyList<MovimientoCajaResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var result = await _repository.GetAllAsync(cancellationToken);
            return result.Select(MapToDTO).ToList().AsReadOnly();
        }

        public async Task<MovimientoCajaResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var movimiento = await _repository.GetByIdAsync(id);
            return movimiento != null ? MapToDTO(movimiento) : null;
        }

        public async Task<IReadOnlyList<MovimientoCajaResponseDto>> GetPorCajaAsync(int cajaId, CancellationToken cancellationToken = default)
        {
            var result = await _repository.GetPorCajaAsync(cajaId);
            return result.Select(MapToDTO).ToList().AsReadOnly();
        }

        public async Task<MovimientoCajaResponseDto> CrearAsync(MovimientoCajaCrearRequest dto, CancellationToken cancellationToken = default)
        {
            var movimiento = new MovimientoCaja
            {
                Monto = dto.Monto,
                Tipo = (TipoMovimiento)dto.TipoMovimiento,
                Concepto = dto.Concepto,
                Fecha = dto.Fecha,
                CajaId = dto.CajaId
            };

            await _repository.AddAsync(movimiento, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return MapToDTO(movimiento);
        }

        public async Task<MovimientoCajaResponseDto?> ActualizarAsync(MovimientoCajaActualizarRequest dto, CancellationToken cancellationToken = default)
        {
            var existente = await _repository.GetByIdAsync(dto.Id);
            if (existente == null) return null;

            existente.Monto = dto.Monto;
            existente.Tipo = (TipoMovimiento)dto.TipoMovimiento;
            existente.Concepto = dto.Concepto;
            existente.Fecha = dto.Fecha;
            existente.CajaId = dto.CajaId;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return MapToDTO(existente);
        }

        public async Task<bool> EliminarAsync(int id, CancellationToken cancellationToken = default)
        {
            var movimiento = await _repository.GetByIdAsync(id);
            if (movimiento == null) return false;

            await _repository.RemoveAsync(movimiento, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }

        private static MovimientoCajaResponseDto MapToDTO(MovimientoCaja movimiento)
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