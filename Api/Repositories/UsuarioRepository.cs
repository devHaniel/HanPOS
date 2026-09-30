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
    /// Repositorio específico para la entidad Usuario.
    /// Implementa IUsuarioRepository usando Entity Framework Core async.
    /// </summary>
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly AppDbContext _context;

        public UsuarioRepository(AppDbContext context) => _context = context;

        public async Task<IReadOnlyList<Usuario>> GetActivosAsync(
            CancellationToken cancellationToken = default)
        {
            return await _context.Usuarios
                .Where(u => u.Activo)
                .ToListAsync(cancellationToken);
        }

        public async Task<Usuario?> GetByUsernameAsync(
            string username,
            CancellationToken cancellationToken = default)
        {
            return await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
        }

        public async Task<bool> UsernameExistsAsync(
            string username,
            CancellationToken cancellationToken = default)
        {
            return await _context.Usuarios
                .AnyAsync(u => u.Username == username, cancellationToken);
        }

        public async Task<Usuario> AddAsync(
            Usuario usuario,
            CancellationToken cancellationToken = default)
        {
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync(cancellationToken);
            return usuario;
        }

        public async Task RemoveAsync(
            Usuario usuario,
            CancellationToken cancellationToken = default)
        {
            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}