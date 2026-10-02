using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Api.Models.Entities;

namespace Api.Services.Interfaces
{
    public interface ITokenService
    {
        string GenerarToken(int idUsuario, IEnumerable<string> roles);
        string GenerarRefreshToken();
        Task<HistorialRefreshToken> GuardarHistorialRefreshToken(int idUsuario, string token, string refreshToken);
        Task<HistorialRefreshToken?> DevolverRefreshToken(string refreshToken);
        Task<bool> RevocarRefreshTokenAsync(HistorialRefreshToken historial);
        ClaimsPrincipal? ObtenerClaimsDesdeTokenExpirado(string token);
    }
}