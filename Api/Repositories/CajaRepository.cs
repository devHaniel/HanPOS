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

        public async Task<Caja?> GetAbiertaAsync(
            CancellationToken cancellationToken = default)
        {
            return await _context.Cajas
                .Where(c => c.EstaAbierta)
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