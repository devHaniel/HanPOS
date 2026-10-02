using System.ComponentModel.DataAnnotations;

namespace Api.DTOs.Auth
{
    /// <summary>
    /// Data Transfer Object for refreshing access token.
    /// </summary>
    public class RefreshTokenDTO
    {
        [Required]
        public string RefreshToken { get; set; } = null!;
    }
}