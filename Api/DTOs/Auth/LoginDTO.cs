using System.ComponentModel.DataAnnotations;

namespace Api.DTOs.Auth
{
    /// <summary>
    /// Data Transfer Object for user login.
    /// </summary>
    public class LoginDTO
    {
        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string Password { get; set; } = null!;
    }
}