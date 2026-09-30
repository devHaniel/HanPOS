using Api.Data;
using Api.Models.Entities;
using Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Repositories
{
    /// <summary>
    /// Repositorio específico para la entidad Cliente.
    /// Implementa IClienteRepository usando Entity Framework Core async.
    /// </summary>
    public class ClienteRepository : IClienteRepository
    {
        private readonly AppDbContext _context;

        public ClienteRepository(AppDbContext context) => _context = context;

        public async Task<IReadOnlyList<Cliente>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            return await _context.Clientes
                .ToListAsync(cancellationToken);
        }

        public async Task<Cliente?> GetWithVentasAsync(
            CancellationToken cancellationToken = default)
        {
            return await _context.Clientes
                .Include(c => c.Ventas)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<Cliente?> GetByRtnAsync(
            string rtn,
            CancellationToken cancellationToken = default)
        {
            return await _context.Clientes
                .FirstOrDefaultAsync(c => c.RTN == rtn, cancellationToken);
        }

        public async Task<Cliente> AddAsync(
            Cliente cliente,
            CancellationToken cancellationToken = default)
        {
            _context.Clientes.Add(cliente);
            return cliente;
        }

        public async Task RemoveAsync(
            Cliente cliente,
            CancellationToken cancellationToken = default)
        {
            _context.Clientes.Remove(cliente);
        }
    }
}