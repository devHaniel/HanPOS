using Api.DTOs;
using Api.DTOs.Auth;
using Api.DTOs.Usuario;
using Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Authenticates a user and returns a JWT token.
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
        /// Registers a new user and returns a JWT token.
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