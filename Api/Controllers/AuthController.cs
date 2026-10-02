using Api.DTOs;
using Api.DTOs.Auth;
using Api.DTOs.Usuario;
using Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Controllers
{
    /// <summary>
    /// Controller for authentication operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [EnableRateLimiting("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Authenticates a user and returns a JWT token with refresh token.
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponseDTO>> Login(
            [FromBody] LoginDTO dto,
            CancellationToken cancellationToken = default)
        {
            var response = await _authService.LoginAsync(dto, cancellationToken);
            if (response == null)
                return Unauthorized(new { message = "Credenciales inválidas" });

            return Ok(response);
        }

        /// <summary>
        /// Registers a new user and returns a JWT token with refresh token.
        /// </summary>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponseDTO>> Register(
            [FromBody] RegisterDTO dto,
            CancellationToken cancellationToken = default)
        {
            var response = await _authService.RegisterAsync(dto, cancellationToken);
            if (response == null)
                return BadRequest(new { message = "El nombre de usuario o email ya existe" });

            return Ok(response);
        }

        /// <summary>
        /// Refreshes the access token using a refresh token.
        /// </summary>
        [HttpPost("refresh-token")]
        [AllowAnonymous]
        [EnableRateLimiting("refresh")]
        public async Task<ActionResult<AuthResponseDTO>> RefreshToken(
            [FromBody] RefreshTokenDTO dto,
            CancellationToken cancellationToken = default)
        {
            var response = await _authService.RefreshTokenAsync(dto, cancellationToken);
            if (response == null)
                return Unauthorized(new { message = "Refresh token inválido o expirado" });

            return Ok(response);
        }

        /// <summary>
        /// Revokes a refresh token (logout).
        /// </summary>
        [HttpPost("revoke-token")]
        [Authorize]
        public async Task<ActionResult> RevokeToken(
            [FromBody] RefreshTokenDTO dto,
            CancellationToken cancellationToken = default)
        {
            var success = await _authService.RevokeRefreshTokenAsync(dto.RefreshToken, cancellationToken);
            if (!success)
                return BadRequest(new { message = "Refresh token inválido" });

            return Ok(new { message = "Token revocado exitosamente" });
        }

        /// <summary>
        /// Changes the current user's password.
        /// </summary>
        [HttpPost("change-password")]
        [Authorize]
        public async Task<ActionResult> ChangePassword(
            [FromBody] ChangePasswordDTO dto,
            CancellationToken cancellationToken = default)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var success = await _authService.ChangePasswordAsync(userId, dto, cancellationToken);
            if (!success)
                return BadRequest(new { message = "Contraseña actual incorrecta" });

            return Ok(new { message = "Contraseña cambiada exitosamente" });
        }

        /// <summary>
        /// Gets the current authenticated user's information.
        /// </summary>
        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<UsuarioDTO>> GetCurrentUser(
            CancellationToken cancellationToken = default)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var usuario = await _authService.GetCurrentUserAsync(userId, cancellationToken);
            if (usuario == null)
                return NotFound();

            return Ok(usuario);
        }
    }
}