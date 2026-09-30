using Api.Data;
using Api.Models.Entities;
using Api.Repositories.Interfaces;
using Api.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Services.Implementations
{
    public class ProveedorService : IProveedorService
    {
        private readonly IProveedorRepository _proveedorRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ProveedorService(IProveedorRepository proveedorRepository, IUnitOfWork unitOfWork)
        {
            _proveedorRepository = proveedorRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IReadOnlyList<Proveedor>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _proveedorRepository.GetAllAsync(cancellationToken);
        }

        public async Task<Proveedor?> GetWithComprasAsync(CancellationToken cancellationToken = default)
        {
            return await _proveedorRepository.GetWithComprasAsync(cancellationToken);
        }

        public async Task<Proveedor?> GetByRtnAsync(string rtn, CancellationToken cancellationToken = default)
        {
            return await _proveedorRepository.GetByRtnAsync(rtn, cancellationToken);
        }

        public async Task<Proveedor> CrearAsync(Proveedor proveedor, CancellationToken cancellationToken = default)
        {
            await _proveedorRepository.AddAsync(proveedor, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return proveedor;
        }

        public async Task<bool> ActualizarAsync(Proveedor proveedor, CancellationToken cancellationToken = default)
        {
            var existente = await _proveedorRepository.GetAllAsync(cancellationToken);
            if (existente == null || existente.Count == 0) return false;

            var first = existente.First();
            first.Nombre = proveedor.Nombre;
            first.RTN = proveedor.RTN;
            first.Telefono = proveedor.Telefono;
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> EliminarAsync(int id, CancellationToken cancellationToken = default)
        {
            var proveedor = await _proveedorRepository.GetAllAsync(cancellationToken);
            var eliminar = proveedor.FirstOrDefault(p => p.Id == id);
            if (eliminar == null) return false;

            await _proveedorRepository.RemoveAsync(eliminar, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}