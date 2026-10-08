using Api.Data;
using Api.Models.Entities;
using Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Repositories
{
    /// <summary>
    /// Repositorio específico para la entidad Negocio.
    /// Implementa INegocioRepository usando Entity Framework Core async.
    /// </summary>
    public class NegocioRepository : INegocioRepository
    {
        private readonly AppDbContext _context;

        public NegocioRepository(AppDbContext context) => _context = context;

        public async Task<Negocio?> GetAsync(CancellationToken cancellationToken = default)
        {
            // El negocio es singleton, debería haber solo un registro con Id = 1
            return await _context.Negocios
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<Negocio?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Negocios
                .FirstOrDefaultAsync(n => n.Id == id, cancellationToken);
        }

        public async Task<Negocio> AddAsync(Negocio negocio, CancellationToken cancellationToken = default)
        {
            _context.Negocios.Add(negocio);
            return negocio;
        }

        public async Task<bool> ExisteAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Negocios.AnyAsync(cancellationToken);
        }
    }
}