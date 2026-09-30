using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Api.Models.Entities;
using Api.Repositories.Interfaces;
using Api.Services.Interfaces;

namespace Api.Services.Implementations
{
    public class CajaService : ICajaService
    {
        private readonly ICajaRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public CajaService(ICajaRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Caja?> GetAbiertaAsync(CancellationToken cancellationToken = default)
        {
            return await _repository.GetAbiertaAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Caja>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _repository.GetAllAsync(cancellationToken);
        }

        public async Task<Caja> CrearAsync(Caja caja, CancellationToken cancellationToken = default)
        {
            await _repository.AddAsync(caja, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return caja;
        }

        public async Task<Caja?> CerrarAsync(int id, decimal montoFinal, CancellationToken cancellationToken = default)
        {
            var caja = await _repository.GetAbiertaAsync(cancellationToken);
            if (caja == null || caja.Id != id) return null;

            caja.FechaCierre = DateTime.Now;
            caja.MontoFinal = montoFinal;

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return caja;
        }

        public async Task<bool> EliminarAsync(int id, CancellationToken cancellationToken = default)
        {
            var caja = await _repository.GetAllAsync(cancellationToken);
            var eliminar = caja.FirstOrDefault(c => c.Id == id);
            if (eliminar == null) return false;

            await _repository.RemoveAsync(eliminar);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
