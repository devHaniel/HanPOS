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

        public async Task<IReadOnlyList<DetalleVenta>> GetPorVentaAsync(
            int ventaId,
            CancellationToken cancellationToken = default)
        {
            return await _context.DetallesVenta
                .Where(d => d.VentaId == ventaId)
                .ToListAsync(cancellationToken);
        }

        public async Task<DetalleVenta> AddAsync(
            DetalleVenta detalle,
            CancellationToken cancellationToken = default)
        {
            _context.DetallesVenta.Add(detalle);
            await _context.SaveChangesAsync(cancellationToken);
            return detalle;
        }

        public async Task RemoveAsync(
            DetalleVenta detalle,
            CancellationToken cancellationToken = default)
        {
            _context.DetallesVenta.Remove(detalle);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}