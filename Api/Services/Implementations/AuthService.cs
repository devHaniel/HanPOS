using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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

        public AuthService(
            UserManager<Usuario> userManager,
            SignInManager<Usuario> signInManager,
            IUsuarioRepository usuarioRepository,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _usuarioRepository = usuarioRepository;
            _configuration = configuration;
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
                PasswordHash = string.Empty, // No devolver el hash
                Activo = usuario.Activo
            };
        }

        private async Task<AuthResponseDTO> GenerateAuthResponseAsync(Usuario usuario)
        {
            var roles = await _userManager.GetRolesAsync(usuario);
            var token = GenerateJwtToken(usuario, roles);
            var expiration = DateTime.UtcNow.AddHours(GetTokenExpirationHours());

            return new AuthResponseDTO
            {
                Token = token,
                Expiration = expiration,
                Usuario = new UsuarioDTO
                {
                    Id = usuario.Id,
                    Nombre = usuario.Nombre,
                    Username = usuario.UserName!,
                    PasswordHash = string.Empty,
                    Activo = usuario.Activo
                }
            };
        }

        private string GenerateJwtToken(Usuario usuario, System.Collections.Generic.IList<string> roles)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new System.Collections.Generic.List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, usuario.UserName!),
                new Claim(JwtRegisteredClaimNames.Email, usuario.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("nombre", usuario.Nombre)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(GetTokenExpirationHours()),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private int GetTokenExpirationHours()
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            return int.TryParse(jwtSettings["ExpirationHours"], out var hours) ? hours : 24;
        }
    }
}