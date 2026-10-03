using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Api.DTOs;
using Api.DTOs.Auth;
using Api.DTOs.Usuario;
using Api.Models.Entities;
using Api.Repositories.Interfaces;
using Api.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Api.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly SignInManager<Usuario> _signInManager;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IConfiguration _configuration;
        private readonly ITokenService _tokenService;

        public AuthService(
            UserManager<Usuario> userManager,
            SignInManager<Usuario> signInManager,
            IUsuarioRepository usuarioRepository,
            IConfiguration configuration,
            ITokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _usuarioRepository = usuarioRepository;
            _configuration = configuration;
            _tokenService = tokenService;
        }

        public async Task<AuthResponseDTO?> LoginAsync(LoginDTO dto, CancellationToken cancellationToken = default)
        {
            var usuario = await _userManager.FindByNameAsync(dto.Username);
            if (usuario == null || !usuario.Activo)
                return null;

            var result = await _signInManager.CheckPasswordSignInAsync(usuario, dto.Password, false);
            if (!result.Succeeded)
                return null;

            return await GenerateAuthResponseAsync(usuario);
        }

        public async Task<AuthResponseDTO?> RegisterAsync(RegisterDTO dto, CancellationToken cancellationToken = default)
        {
            // Verificar si el username ya existe
            var existingUser = await _userManager.FindByNameAsync(dto.Username);
            if (existingUser != null)
                return null;

            // Verificar si el email ya existe
            var existingEmail = await _userManager.FindByEmailAsync(dto.Email);
            if (existingEmail != null)
                return null;

            var usuario = new Usuario
            {
                UserName = dto.Username,
                Email = dto.Email,
                Nombre = dto.Nombre,
                Activo = true,
                EmailConfirmed = true // Para desarrollo, en producción debería ser false
            };

            var result = await _userManager.CreateAsync(usuario, dto.Password);
            if (!result.Succeeded)
                return null;

            // Asignar rol por defecto (opcional)
            await _userManager.AddToRoleAsync(usuario, "Vendedor");

            return await GenerateAuthResponseAsync(usuario);
        }

        public async Task<AuthResponseDTO?> RefreshTokenAsync(RefreshTokenDTO dto, CancellationToken cancellationToken = default)
        {
            // Obtener el historial del refresh token
            var historial = await _tokenService.DevolverRefreshToken(dto.RefreshToken);
            if (historial == null)
                return null;

            // Obtener el usuario
            var usuario = await _userManager.FindByIdAsync(historial.UsuarioId.ToString());
            if (usuario == null || !usuario.Activo)
                return null;

            // Revocar el refresh token actual
            await _tokenService.RevocarRefreshTokenAsync(historial);

            // Generar nueva respuesta de autenticación
            return await GenerateAuthResponseAsync(usuario);
        }

        public async Task<bool> RevokeRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
        {
            var historial = await _tokenService.DevolverRefreshToken(refreshToken);
            if (historial == null)
                return false;

            return await _tokenService.RevocarRefreshTokenAsync(historial);
        }

        public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordDTO dto, CancellationToken cancellationToken = default)
        {
            var usuario = await _userManager.FindByIdAsync(userId.ToString());
            if (usuario == null)
                return false;

            var result = await _userManager.ChangePasswordAsync(usuario, dto.CurrentPassword, dto.NewPassword);
            return result.Succeeded;
        }

        public async Task<UsuarioDTO?> GetCurrentUserAsync(int userId, CancellationToken cancellationToken = default)
        {
            var usuario = await _userManager.FindByIdAsync(userId.ToString());
            if (usuario == null)
                return null;

            return new UsuarioDTO
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Username = usuario.UserName!,
                Role = (await _userManager.GetRolesAsync(usuario)).FirstOrDefault(),
                PasswordHash = string.Empty, // No devolver el hash
                Activo = usuario.Activo
            };
        }

        private async Task<AuthResponseDTO> GenerateAuthResponseAsync(Usuario usuario)
        {
            var roles = await _userManager.GetRolesAsync(usuario);
            var token = _tokenService.GenerarToken(usuario.Id, roles);
            var refreshToken = _tokenService.GenerarRefreshToken();
            var expiration = DateTime.UtcNow.AddMinutes(GetTokenExpirationMinutes());

            // Guardar historial de refresh token
            await _tokenService.GuardarHistorialRefreshToken(usuario.Id, token, refreshToken);

            return new AuthResponseDTO
            {
                Token = token,
                RefreshToken = refreshToken,
                Expiration = expiration,
                Usuario = new UsuarioDTO
                {
                    Id = usuario.Id,
                    Nombre = usuario.Nombre,
                    Username = usuario.UserName!,
                    Role = roles.FirstOrDefault(),
                    PasswordHash = string.Empty,
                    Activo = usuario.Activo
                }
            };
        }

        private int GetTokenExpirationMinutes()
        {
            return _configuration.GetValue<int>("Jwt:AccessTokenMinutes", 60);
        }
    }
}