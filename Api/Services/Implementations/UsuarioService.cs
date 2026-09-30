using Api.Data;
using Api.Models.Entities;
using Api.Repositories.Interfaces;
using Api.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Services.Implementations
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UsuarioService(IUsuarioRepository usuarioRepository, IUnitOfWork unitOfWork)
        {
            _usuarioRepository = usuarioRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IReadOnlyList<Usuario>> GetActivosAsync(CancellationToken cancellationToken = default)
        {
            return await _usuarioRepository.GetActivosAsync(cancellationToken);
        }

        public async Task<Usuario> CrearAsync(Usuario usuario, CancellationToken cancellationToken = default)
        {
            // Validar que el username sea único antes de crear
            var existe = await _usuarioRepository.UsernameExistsAsync(usuario.Username, cancellationToken);
            if (existe) throw new System.Exception("El nombre de usuario ya existe");

            await _usuarioRepository.AddAsync(usuario, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return usuario;
        }

        public async Task<bool> ActualizarAsync(Usuario usuario, CancellationToken cancellationToken = default)
        {
            var existente = await _usuarioRepository.GetActivosAsync(cancellationToken);
            if (existente == null || existente.Count == 0) return false;

            var first = existente.First();
            first.Nombre = usuario.Nombre;
            first.Username = usuario.Username;
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> EliminarAsync(int id, CancellationToken cancellationToken = default)
        {
            var usuario = await _usuarioRepository.GetActivosAsync(cancellationToken);
            var eliminar = usuario.FirstOrDefault(u => u.Id == id);
            if (eliminar == null) return false;

            await _usuarioRepository.RemoveAsync(eliminar, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}