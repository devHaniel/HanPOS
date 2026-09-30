using Api.Data;
using Api.Models.Entities;
using Api.Repositories;
using Api.Repositories.Interfaces;
using Api.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Api.Services.Implementations
{
    public class CategoriaService : ICategoriaService
    {
        private readonly ICategoriaRepository _categoriaRepository;

        public CategoriaService(ICategoriaRepository categoriaRepository) => _categoriaRepository = categoriaRepository;

        public async Task<IReadOnlyList<Categoria>> GetActivosAsync(
            CancellationToken cancellationToken = default)
        {
            return await _categoriaRepository.GetActivasAsync(cancellationToken);
        }

        public async Task<Categoria> CrearAsync(Categoria categoria, CancellationToken cancellationToken = default)
        {
            categoria.Activo = true;
            await _categoriaRepository.AddAsync(categoria, cancellationToken);
            var rows = await _categoriaRepository.SaveChangesAsync(cancellationToken);
            if (rows <= 0) throw new System.Exception("Error al guardar categoría");
            return categoria;
        }

        public async Task<bool> ActualizarAsync(Categoria categoria, CancellationToken cancellationToken = default)
        {
            var existente = await _categoriaRepository.GetActivosAsync(cancellationToken);
            if (existente == null || existente.Count == 0) return false;

            var first = existente.First();
            first.Nombre = categoria.Nombre;
            first.Activo = categoria.Activo;
            await _categoriaRepository.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> EliminarAsync(int id, CancellationToken cancellationToken = default)
            => await Task.FromResult(false);

        public Task<IReadOnlyList<Categoria>> GetActivasAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}