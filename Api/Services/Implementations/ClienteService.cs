using Api.DTOs.Cliente;
using Api.DTOs.Venta;
using Api.Models.Entities;
using Api.Repositories.Interfaces;
using Api.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Services.Implementations
{
    public class ClienteService : IClienteService
    {
        private readonly IClienteRepository _clienteRepository;
        private readonly IVentaRepository _ventaRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ClienteService(IClienteRepository clienteRepository, IVentaRepository ventaRepository, IUnitOfWork unitOfWork)
        {
            _clienteRepository = clienteRepository;
            _ventaRepository = ventaRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IReadOnlyList<ClienteResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var clientes = await _clienteRepository.GetAllAsync(cancellationToken);
            return clientes.Select(MapToDTO).ToList().AsReadOnly();
        }

        public async Task<ClienteResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var cliente = await _clienteRepository.GetByIdAsync(id, cancellationToken);
            return cliente != null ? MapToDTO(cliente) : null;
        }

        public async Task<ClienteResponseDto?> GetByRtnAsync(string rtn, CancellationToken cancellationToken = default)
        {
            var cliente = await _clienteRepository.GetByRtnAsync(rtn, cancellationToken);
            return cliente != null ? MapToDTO(cliente) : null;
        }

        public async Task<IReadOnlyList<VentaResponseDto>> GetVentasAsync(int clienteId, CancellationToken cancellationToken = default)
        {
            var all = await _ventaRepository.GetAllAsync(cancellationToken);
            return all.Where(v => v.ClienteId == clienteId).Select(MapToVentaDTO).ToList().AsReadOnly();
        }

        public async Task<ClienteResponseDto> CrearAsync(ClienteCrearRequest dto, CancellationToken cancellationToken = default)
        {
            var cliente = new Cliente
            {
                Nombre = dto.Nombre,
                Telefono = dto.Telefono,
                RTN = dto.RTN
            };

            await _clienteRepository.AddAsync(cliente, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return MapToDTO(cliente);
        }

        public async Task<ClienteResponseDto?> ActualizarAsync(ClienteActualizarRequest dto, CancellationToken cancellationToken = default)
        {
            var existente = await _clienteRepository.GetByIdAsync(dto.Id, cancellationToken);
            if (existente == null) return null;

            existente.Nombre = dto.Nombre;
            existente.Telefono = dto.Telefono;
            existente.RTN = dto.RTN;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return MapToDTO(existente);
        }

        public async Task<bool> EliminarAsync(int id, CancellationToken cancellationToken = default)
        {
            var cliente = await _clienteRepository.GetByIdAsync(id, cancellationToken);
            if (cliente == null) return false;

            await _clienteRepository.RemoveAsync(cliente, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }

        private static ClienteResponseDto MapToDTO(Cliente cliente)
        {
            return new ClienteResponseDto
            {
                Id = cliente.Id,
                Nombre = cliente.Nombre,
                Telefono = cliente.Telefono,
                RTN = cliente.RTN
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
    }
}