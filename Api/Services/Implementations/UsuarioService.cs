using Api.DTOs.Usuario;
using Api.DTOs.Venta;
using Api.DTOs.Compra;
using Api.DTOs.Caja;
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
        private readonly IVentaRepository _ventaRepository;
        private readonly ICompraRepository _compraRepository;
        private readonly ICajaRepository _cajaRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UsuarioService(
            IUsuarioRepository usuarioRepository,
            IVentaRepository ventaRepository,
            ICompraRepository compraRepository,
            ICajaRepository cajaRepository,
            IUnitOfWork unitOfWork)
        {
            _usuarioRepository = usuarioRepository;
            _ventaRepository = ventaRepository;
            _compraRepository = compraRepository;
            _cajaRepository = cajaRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IReadOnlyList<UsuarioResponseDto>> GetActivosAsync(CancellationToken cancellationToken = default)
        {
            var usuarios = await _usuarioRepository.GetActivosAsync(cancellationToken);
            return usuarios.Select(MapToDTO).ToList().AsReadOnly();
        }

        public async Task<UsuarioResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id, cancellationToken);
            return usuario != null ? MapToDTO(usuario) : null;
        }

        public async Task<UsuarioResponseDto?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
        {
            var usuario = await _usuarioRepository.GetByUsernameAsync(username, cancellationToken);
            return usuario != null ? MapToDTO(usuario) : null;
        }

        public async Task<IReadOnlyList<VentaResponseDto>> GetVentasAsync(int usuarioId, CancellationToken cancellationToken = default)
        {
            var all = await _ventaRepository.GetAllAsync(cancellationToken);
            return all.Where(v => v.UsuarioId == usuarioId).Select(MapToVentaDTO).ToList().AsReadOnly();
        }

        public async Task<IReadOnlyList<CompraResponseDto>> GetComprasAsync(int usuarioId, CancellationToken cancellationToken = default)
        {
            var all = await _compraRepository.GetAllAsync(cancellationToken);
            return all.Where(c => c.UsuarioId == usuarioId).Select(MapToCompraDTO).ToList().AsReadOnly();
        }

        public async Task<IReadOnlyList<CajaResponseDto>> GetCajasAsync(int usuarioId, CancellationToken cancellationToken = default)
        {
            var all = await _cajaRepository.GetAllAsync(cancellationToken);
            return all.Where(c => c.UsuarioId == usuarioId).Select(MapToCajaDTO).ToList().AsReadOnly();
        }

        public async Task<UsuarioResponseDto> CrearAsync(UsuarioCrearRequest dto, CancellationToken cancellationToken = default)
        {
            var existe = await _usuarioRepository.UsernameExistsAsync(dto.Username, cancellationToken);
            if (existe) throw new System.Exception("El nombre de usuario ya existe");

            var emailExiste = await _usuarioRepository.EmailExistsAsync(dto.Email, cancellationToken);
            if (emailExiste) throw new System.Exception("El email ya existe");

            var usuario = new Usuario
            {
                Nombre = dto.Nombre,
                UserName = dto.Username,
                Email = dto.Email,
                PasswordHash = dto.Password, // El controlador/AuthService debe hashear esto
                Activo = dto.Activo
            };

            await _usuarioRepository.AddAsync(usuario, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return MapToDTO(usuario);
        }

        public async Task<UsuarioResponseDto?> ActualizarAsync(UsuarioActualizarRequest dto, CancellationToken cancellationToken = default)
        {
            var existente = await _usuarioRepository.GetByIdAsync(dto.Id, cancellationToken);
            if (existente == null) return null;

            // Validar username único (excluyendo el actual)
            var usernameExiste = await _usuarioRepository.GetByUsernameAsync(dto.Username, cancellationToken);
            if (usernameExiste != null && usernameExiste.Id != dto.Id)
                throw new System.Exception("El nombre de usuario ya existe");

            // Validar email único (excluyendo el actual)
            var emailExiste = await _usuarioRepository.GetByEmailAsync(dto.Email, cancellationToken);
            if (emailExiste != null && emailExiste.Id != dto.Id)
                throw new System.Exception("El email ya existe");

            existente.Nombre = dto.Nombre;
            existente.UserName = dto.Username;
            existente.Email = dto.Email;
            if (!string.IsNullOrEmpty(dto.Password))
                existente.PasswordHash = dto.Password; // El controlador/AuthService debe hashear esto
            existente.Activo = dto.Activo;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return MapToDTO(existente);
        }

        public async Task<bool> EliminarAsync(int id, CancellationToken cancellationToken = default)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id, cancellationToken);
            if (usuario == null) return false;

            usuario.Activo = false; // Soft delete
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }

        private static UsuarioResponseDto MapToDTO(Usuario usuario)
        {
            return new UsuarioResponseDto
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                UserName = usuario.UserName!,
                Email = usuario.Email!,
                Activo = usuario.Activo
            };
        }

        private static VentaResponseDto MapToVentaDTO(Venta venta)
        {
            return new VentaResponseDto
            {
                Id = venta.Id,
                Fecha = venta.Fecha,
                Subtotal = venta.Subtotal,
                Impuesto = venta.Impuesto,
                Total = venta.Total,
                MetodoPago = (int?)venta.MetodoPago,
                Estado = (int?)venta.Estado,
                UsuarioId = venta.UsuarioId,
                CajaId = venta.CajaId,
                ClienteId = venta.ClienteId
            };
        }

        private static CompraResponseDto MapToCompraDTO(Compra compra)
        {
            return new CompraResponseDto
            {
                Id = compra.Id,
                Fecha = compra.Fecha,
                Subtotal = compra.Subtotal,
                Impuesto = compra.Impuesto,
                Total = compra.Total,
                Estado = (int)compra.Estado,
                UsuarioId = compra.UsuarioId,
                CajaId = compra.CajaId,
                ProveedorId = compra.ProveedorId
            };
        }

        private static CajaResponseDto MapToCajaDTO(Caja caja)
        {
            return new CajaResponseDto
            {
                Id = caja.Id,
                FechaApertura = caja.FechaApertura,
                FechaCierre = caja.FechaCierre,
                MontoInicial = caja.MontoInicial,
                MontoFinal = caja.MontoFinal,
                UsuarioId = caja.UsuarioId
            };
        }
    }
}