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
    /// Repositorio específico para la entidad Categoria.
    /// Implementa ICategoriaRepository usando Entity Framework Core async.
    /// </summary>
    public class CategoriaRepository : ICategoriaRepository
    {
        private readonly AppDbContext _context;

        public CategoriaRepository(AppDbContext context) => _context = context;

        public async Task<IReadOnlyList<Categoria>> GetActivasAsync(
            CancellationToken cancellationToken = default)
        {
            return await _context.Categorias
                .Where(c => c.Activo)
                .ToListAsync(cancellationToken);
        }

        public async Task<Categoria?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            return await _context.Categorias
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        public async Task<Categoria?> GetWithProductosAsync(
            CancellationToken cancellationToken = default)
        {
            return await _context.Categorias
                .Include(c => c.Productos)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<Categoria> AddAsync(
            Categoria categoria,
            CancellationToken cancellationToken = default)
        {
            _context.Categorias.Add(categoria);
            return categoria;
        }

        public async Task RemoveAsync(
            Categoria categoria,
            CancellationToken cancellationToken = default)
        {
            _context.Categorias.Remove(categoria);
        }
    }
}