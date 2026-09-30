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
    /// Repositorio específico para la entidad MovimientoCaja.
    /// Implementa IMovimientoCajaRepository usando Entity Framework Core async.
    /// </summary>
    public class MovimientoCajaRepository : IMovimientoCajaRepository
    {
        private readonly AppDbContext _context;

        public MovimientoCajaRepository(AppDbContext context) => _context = context;

        public async Task<IReadOnlyList<MovimientoCaja>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            return await _context.MovimientosCaja
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<MovimientoCaja>> GetPorCajaAsync(
            int cajaId,
            CancellationToken cancellationToken = default)
        {
            return await _context.MovimientosCaja
                .Where(m => m.CajaId == cajaId)
                .ToListAsync(cancellationToken);
        }

        public async Task<MovimientoCaja> AddAsync(
            MovimientoCaja movimiento,
            CancellationToken cancellationToken = default)
        {
            _context.MovimientosCaja.Add(movimiento);
            await _context.SaveChangesAsync(cancellationToken);
            return movimiento;
        }

        public async Task RemoveAsync(
            MovimientoCaja movimiento,
            CancellationToken cancellationToken = default)
        {
            _context.MovimientosCaja.Remove(movimiento);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<MovimientoCaja?> GetPorId(int id)
        {
            return await _context.MovimientosCaja
                .FirstOrDefaultAsync(m => m.Id == id);
        }
    }
}