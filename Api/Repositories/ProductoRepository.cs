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
    /// Repositorio específico para la entidad Producto.
    /// Implementa IProductoRepository usando Entity Framework Core async.
    /// </summary>
    public class ProductoRepository : IProductoRepository
    {
        private readonly AppDbContext _context;

        public ProductoRepository(AppDbContext context) => _context = context;

        public async Task<IReadOnlyList<Producto>> GetActivosAsync(
            CancellationToken cancellationToken = default)
        {
            return await _context.Productos
                .Where(p => p.Activo)
                .ToListAsync(cancellationToken);
        }

        public async Task<Producto?> GetById(
            int id,
            CancellationToken cancellationToken = default)
        {
            return await _context.Productos
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyList<Producto>> GetPorCategoriaAsync(
            int categoriaId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Productos
                .Where(p => p.CategoriaId == categoriaId && p.Activo)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Producto>> StockBajoAsync(
            CancellationToken cancellationToken = default)
        {
            return await _context.Productos
                .Where(p => p.Stock <= p.StockMinimo && p.Activo)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Producto>> BuscarPorNombreAsync(
            string termino,
            CancellationToken cancellationToken = default)
        {
            return await _context.Productos
                .Where(p => p.Nombre.Contains(termino) && p.Activo)
                .ToListAsync(cancellationToken);
        }

        public async Task<Producto> AddAsync(
            Producto producto,
            CancellationToken cancellationToken = default)
        {
            _context.Productos.Add(producto);
            return producto;
        }

        public async Task RemoveAsync(
            Producto producto,
            CancellationToken cancellationToken = default)
        {
            producto.Activo = false;
        }

        public async Task<Producto?> GetByCodigo(string codigo, CancellationToken cancellationToken = default)
        {
            return await _context.Productos
                .FirstOrDefaultAsync(p => p.Codigo.Contains(codigo) , cancellationToken);
        }
    }
}