using Api.Data;
using Api.Models.Entities;
using Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Api.Repositories
{
    /// <summary>
    /// Repositorio específico para la entidad Compra.
    /// Implementa ICompraRepository usando Entity Framework Core async.
    /// </summary>
    public class CompraRepository : ICompraRepository
    {
        private readonly AppDbContext _context;

        public CompraRepository(AppDbContext context) => _context = context;

        public async Task<IReadOnlyList<Compra>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            return await _context.Compras
                .ToListAsync(cancellationToken);
        }

        public async Task<Compra?> GetWithDetailsAndProveedorAsync(
            int compraId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Compras
                .Where(c => c.Id == compraId)
                .Include(c => c.Detalles)
                    .ThenInclude(d => d.Producto)
                .Include(c => c.Proveedor)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<Compra> AddAsync(
            Compra compra,
            CancellationToken cancellationToken = default)
        {
            _context.Compras.Add(compra);
            return compra;
        }

        public async Task RemoveAsync(
            Compra compra,
            CancellationToken cancellationToken = default)
        {
            _context.Compras.Remove(compra);
        }
    }
}