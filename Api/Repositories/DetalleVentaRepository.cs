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
    /// Repositorio específico para la entidad DetalleVenta.
    /// Implementa IDetalleVentaRepository usando Entity Framework Core async.
    /// </summary>
    public class DetalleVentaRepository : IDetalleVentaRepository
    {
        private readonly AppDbContext _context;

        public DetalleVentaRepository(AppDbContext context) => _context = context;

        public async Task<IReadOnlyList<DetalleVenta>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            return await _context.DetallesVenta
                .ToListAsync(cancellationToken);
        }

        public async Task<DetalleVenta?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            return await _context.DetallesVenta
                .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyList<DetalleVenta>> GetPorVentaAsync(
            int ventaId,
            CancellationToken cancellationToken = default)
        {
            return await _context.DetallesVenta
                .Where(d => d.VentaId == ventaId)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<DetalleVenta>> GetByVentaIdAsync(
            int ventaId,
            CancellationToken cancellationToken = default)
        {
            return await GetPorVentaAsync(ventaId, cancellationToken);
        }

        public async Task<IReadOnlyList<DetalleVenta>> GetByProductoIdAsync(
            int productoId,
            CancellationToken cancellationToken = default)
        {
            return await _context.DetallesVenta
                .Where(d => d.ProductoId == productoId)
                .ToListAsync(cancellationToken);
        }

        public async Task<DetalleVenta> AddAsync(
            DetalleVenta detalle,
            CancellationToken cancellationToken = default)
        {
            _context.DetallesVenta.Add(detalle);
            return detalle;
        }

        public async Task RemoveAsync(
            DetalleVenta detalle,
            CancellationToken cancellationToken = default)
        {
            _context.DetallesVenta.Remove(detalle);
        }
    }
}