using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Models.Entities;
using Api.Repositories.Interfaces;
using Api.Services.Interfaces;

namespace Api.Services.Implementations
{
    public class MovimientoCajaService: IMovimientoCajaService
    {
        private readonly IMovimientoCajaRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public MovimientoCajaService(IMovimientoCajaRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public Task<MovimientoCaja> AddAsync(MovimientoCaja movimiento, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<IReadOnlyList<MovimientoCaja>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var result = await _repository.GetAllAsync();

            return result;
        }

        public async Task<IReadOnlyList<MovimientoCaja>> GetPorCajaAsync(int cajaId, CancellationToken cancellationToken = default)
        {
            var result = await _repository.GetPorCajaAsync(cajaId);

            return result;
        }

        public async Task<MovimientoCaja?> GetPorId(int id)
        {
            return await _repository.GetPorId(id);
        }

        public async Task RemoveAsync(MovimientoCaja movimiento, CancellationToken cancellationToken = default)
        {
            if(movimiento == null)
                throw new ArgumentNullException("Movimiento no pasado.");
            var buscarMovimiento = await _repository.GetPorId(movimiento.Id);

            if(buscarMovimiento == null)
                throw new ArgumentException("Movimiento no encontrado.");

            await _repository.RemoveAsync(movimiento);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}