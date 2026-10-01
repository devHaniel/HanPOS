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
                .Include(v => v.Detalles)
                .Include(v => v.Cliente)
                .ToListAsync(cancellationToken);
        }

        public async Task<(List<Venta> Items, int Total)> GetAllPagedAsync(
            int pagina,
            int cantidad,
            CancellationToken cancellationToken = default)
        {
            var query = _context.Ventas
                .Include(v => v.Detalles)
                .Include(v => v.Cliente)
                .AsQueryable();
            var total = await query.CountAsync(cancellationToken);
            var items = await query
                .OrderByDescending(v => v.Fecha)
                .Skip((pagina - 1) * cantidad)
                .Take(cantidad)
                .ToListAsync(cancellationToken);
            return (items, total);
        }

        public async Task<Venta?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            return await _context.Ventas
                .Include(v => v.Detalles)
                .Include(v => v.Cliente)
                .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);
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
            return venta;
        }

        public async Task RemoveAsync(
            Venta venta,
            CancellationToken cancellationToken = default)
        {
            _context.Ventas.Remove(venta);
        }
    }
}