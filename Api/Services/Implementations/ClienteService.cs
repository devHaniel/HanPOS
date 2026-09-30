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
    public class ClienteService : IClienteService
    {
        private readonly IClienteRepository _clienteRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ClienteService(IClienteRepository clienteRepository, IUnitOfWork unitOfWork)
        {
            _clienteRepository = clienteRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IReadOnlyList<Cliente>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _clienteRepository.GetAllAsync(cancellationToken);
        }

        public async Task<Cliente?> GetWithVentasAsync(CancellationToken cancellationToken = default)
        {
            return await _clienteRepository.GetWithVentasAsync(cancellationToken);
        }

        public async Task<Cliente?> GetByRtnAsync(string rtn, CancellationToken cancellationToken = default)
        {
            return await _clienteRepository.GetByRtnAsync(rtn, cancellationToken);
        }

        public async Task<Cliente> CrearAsync(Cliente cliente, CancellationToken cancellationToken = default)
        {
            await _clienteRepository.AddAsync(cliente, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return cliente;
        }

        public async Task<bool> ActualizarAsync(Cliente cliente, CancellationToken cancellationToken = default)
        {
            var existente = await _clienteRepository.GetAllAsync(cancellationToken);
            if (existente == null || existente.Count == 0) return false;

            var first = existente.First(c => c.Id != cliente.Id);
            // Actualizar campos simples - en un caso real haríamos un mapeo proper
            first.Nombre = cliente.Nombre;
            first.RTN = cliente.RTN;
            first.Telefono = cliente.Telefono;
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> EliminarAsync(int id, CancellationToken cancellationToken = default)
        {
            var cliente = await _clienteRepository.GetAllAsync(cancellationToken);
            var eliminar = cliente.FirstOrDefault(c => c.Id == id);
            if (eliminar == null) return false;

            await _clienteRepository.RemoveAsync(eliminar, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}