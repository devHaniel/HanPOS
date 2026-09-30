using Api.Data;
using Api.Models.Entities;
using Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System;

namespace Api.Repositories
{
    /// <summary>
    /// Repositorio específico para la entidad Venta.
    /// Implementa IVentaRepository usando Entity Framework Core async.
    /// </summary>
    public class VentaRepository : IVentaRepository
    {
        private readonly AppDbContext _context;

        public VentaRepository(AppDbContext context) => _context = context;

        public async Task<IReadOnlyList<Venta>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            return await _context.Ventas
                .ToListAsync(cancellationToken);
        }

        public async Task<Venta?> GetWithDetailsAndClienteAsync(
            int ventaId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Ventas
                .Where(v => v.Id == ventaId)
                .Include(v => v.Detalles)
                    .ThenInclude(d => d.Producto)
                .Include(v => v.Cliente)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<Venta> AddAsync(
            Venta venta,
            CancellationToken cancellationToken = default)
        {
            _context.Ventas.Add(venta);
            await _context.SaveChangesAsync(cancellationToken);
            return venta;
        }

        public async Task RemoveAsync(
            Venta venta,
            CancellationToken cancellationToken = default)
        {
            _context.Ventas.Remove(venta);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}