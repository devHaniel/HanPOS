using Api.Data;
using Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Repositories
{
    /// <summary>
    /// Unit of Work minimalista. Solo gestiona el SaveChangesAsync.
    /// Los repositorios operan directamente sobre el DbContext.
    /// El servicio será quien decida cuándo llamar a SaveChanges.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public UnitOfWork(AppDbContext context) => _context = context;

        /// <summary>
        /// Guarda los cambios asíncronamente.
        /// Este es el único punto donde se persisten los datos.
        /// </summary>
        public async Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
