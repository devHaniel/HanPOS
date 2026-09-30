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
    /// Repositorio específico para la entidad DetalleCompra.
    /// Implementa IDetalleCompraRepository usando Entity Framework Core async.
    /// </summary>
    public class DetalleCompraRepository : IDetalleCompraRepository
    {
        private readonly AppDbContext _context;

        public DetalleCompraRepository(AppDbContext context) => _context = context;

        public async Task<IReadOnlyList<DetalleCompra>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            return await _context.DetallesCompra
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<DetalleCompra>> GetPorCompraAsync(
            int compraId,
            CancellationToken cancellationToken = default)
        {
            return await _context.DetallesCompra
                .Where(d => d.CompraId == compraId)
                .ToListAsync(cancellationToken);
        }

        public async Task<DetalleCompra> AddAsync(
            DetalleCompra detalle,
            CancellationToken cancellationToken = default)
        {
            _context.DetallesCompra.Add(detalle);
            await _context.SaveChangesAsync(cancellationToken);
            return detalle;
        }

        public async Task RemoveAsync(
            DetalleCompra detalle,
            CancellationToken cancellationToken = default)
        {
            _context.DetallesCompra.Remove(detalle);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}