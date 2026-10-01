using Api.DTOs.Proveedor;
using Api.DTOs.Compra;
using Api.Models.Entities;
using Api.Repositories.Interfaces;
using Api.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Services.Implementations
{
    public class ProveedorService : IProveedorService
    {
        private readonly IProveedorRepository _proveedorRepository;
        private readonly ICompraRepository _compraRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ProveedorService(IProveedorRepository proveedorRepository, ICompraRepository compraRepository, IUnitOfWork unitOfWork)
        {
            _proveedorRepository = proveedorRepository;
            _compraRepository = compraRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IReadOnlyList<ProveedorResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var proveedores = await _proveedorRepository.GetAllAsync(cancellationToken);
            return proveedores.Select(MapToDTO).ToList().AsReadOnly();
        }

        public async Task<ProveedorResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var proveedor = await _proveedorRepository.GetByIdAsync(id, cancellationToken);
            return proveedor != null ? MapToDTO(proveedor) : null;
        }

        public async Task<ProveedorResponseDto?> GetByRtnAsync(string rtn, CancellationToken cancellationToken = default)
        {
            var proveedor = await _proveedorRepository.GetByRtnAsync(rtn, cancellationToken);
            return proveedor != null ? MapToDTO(proveedor) : null;
        }

        public async Task<IReadOnlyList<CompraResponseDto>> GetComprasAsync(int proveedorId, CancellationToken cancellationToken = default)
        {
            var all = await _compraRepository.GetAllAsync(cancellationToken);
            return all.Where(c => c.ProveedorId == proveedorId).Select(MapToCompraDTO).ToList().AsReadOnly();
        }

        public async Task<ProveedorResponseDto> CrearAsync(ProveedorCrearRequest dto, CancellationToken cancellationToken = default)
        {
            var proveedor = new Proveedor
            {
                Nombre = dto.Nombre,
                Telefono = dto.Telefono,
                RTN = dto.RTN
            };

            await _proveedorRepository.AddAsync(proveedor, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return MapToDTO(proveedor);
        }

        public async Task<ProveedorResponseDto?> ActualizarAsync(ProveedorActualizarRequest dto, CancellationToken cancellationToken = default)
        {
            var existente = await _proveedorRepository.GetByIdAsync(dto.Id, cancellationToken);
            if (existente == null) return null;

            existente.Nombre = dto.Nombre;
            existente.Telefono = dto.Telefono;
            existente.RTN = dto.RTN;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return MapToDTO(existente);
        }

        public async Task<bool> EliminarAsync(int id, CancellationToken cancellationToken = default)
        {
            var proveedor = await _proveedorRepository.GetByIdAsync(id, cancellationToken);
            if (proveedor == null) return false;

            await _proveedorRepository.RemoveAsync(proveedor, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }

        private static ProveedorResponseDto MapToDTO(Proveedor proveedor)
        {
            return new ProveedorResponseDto
            {
                Id = proveedor.Id,
                Nombre = proveedor.Nombre,
                Telefono = proveedor.Telefono,
                RTN = proveedor.RTN
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
    }
}