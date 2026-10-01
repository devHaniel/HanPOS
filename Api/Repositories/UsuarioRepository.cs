using Api.Data;
using Api.Models.Entities;
using Api.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
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
            return await _context.Users
                .Where(u => u.Activo)
                .ToListAsync(cancellationToken);
        }

        public async Task<Usuario?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        }

        public async Task<Usuario?> GetByUsernameAsync(
            string username,
            CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == username, cancellationToken);
        }

        public async Task<Usuario?> GetByEmailAsync(
            string email,
            CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        }

        public async Task<bool> UsernameExistsAsync(
            string username,
            CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .AnyAsync(u => u.UserName == username, cancellationToken);
        }

        public async Task<bool> EmailExistsAsync(
            string email,
            CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .AnyAsync(u => u.Email == email, cancellationToken);
        }

        public async Task<Usuario> AddAsync(
            Usuario usuario,
            CancellationToken cancellationToken = default)
        {
            _context.Users.Add(usuario);
            return usuario;
        }

        public async Task RemoveAsync(
            Usuario usuario,
            CancellationToken cancellationToken = default)
        {
            _context.Users.Remove(usuario);
        }
    }
}