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
    /// Repositorio específico para la entidad Caja.
    /// Implementa ICajaRepository usando Entity Framework Core async.
    /// </summary>
    public class CajaRepository : ICajaRepository
    {
        private readonly AppDbContext _context;

        public CajaRepository(AppDbContext context) => _context = context;

        public async Task<IReadOnlyList<Caja>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            return await _context.Cajas
                .ToListAsync(cancellationToken);
        }

        public async Task<(List<Caja> Items, int Total)> GetAllPagedAsync(
            int pagina,
            int cantidad,
            CancellationToken cancellationToken = default)
        {
            var query = _context.Cajas.AsQueryable();
            var total = await query.CountAsync(cancellationToken);
            var items = await query
                .OrderByDescending(c => c.FechaApertura)
                .Skip((pagina - 1) * cantidad)
                .Take(cantidad)
                .ToListAsync(cancellationToken);
            return (items, total);
        }

        public async Task<Caja?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            return await _context.Cajas
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        public async Task<Caja?> GetAbiertaAsync(
            CancellationToken cancellationToken = default)
        {
            return await _context.Cajas
                .Where(c => c.FechaCierre == null)
                .OrderByDescending(c => c.FechaApertura)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<Caja> AddAsync(
            Caja caja,
            CancellationToken cancellationToken = default)
        {
            await _context.Cajas.AddAsync(caja, cancellationToken);
            return caja;
        }

        public async Task RemoveAsync(
            Caja caja,
            CancellationToken cancellationToken = default)
        {
            _context.Cajas.Remove(caja);
        }
    }
}