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
    /// Repositorio específico para la entidad Proveedor.
    /// Implementa IProveedorRepository usando Entity Framework Core async.
    /// </summary>
    public class ProveedorRepository : IProveedorRepository
    {
        private readonly AppDbContext _context;

        public ProveedorRepository(AppDbContext context) => _context = context;

        public async Task<IReadOnlyList<Proveedor>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            return await _context.Proveedores
                .ToListAsync(cancellationToken);
        }

        public async Task<Proveedor?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            return await _context.Proveedores
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<Proveedor?> GetWithComprasAsync(
            CancellationToken cancellationToken = default)
        {
            return await _context.Proveedores
                .Include(p => p.Compras)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<Proveedor?> GetByRtnAsync(
            string rtn,
            CancellationToken cancellationToken = default)
        {
            return await _context.Proveedores
                .FirstOrDefaultAsync(p => p.RTN == rtn, cancellationToken);
        }

        public async Task<Proveedor> AddAsync(
            Proveedor proveedor,
            CancellationToken cancellationToken = default)
        {
            _context.Proveedores.Add(proveedor);
            return proveedor;
        }

        public async Task RemoveAsync(
            Proveedor proveedor,
            CancellationToken cancellationToken = default)
        {
            _context.Proveedores.Remove(proveedor);
        }
    }
}