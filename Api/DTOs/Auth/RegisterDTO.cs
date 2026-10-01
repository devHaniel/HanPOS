using System.ComponentModel.DataAnnotations;

namespace Api.DTOs.Auth
{
    /// <summary>
    /// Data Transfer Object for user registration.
    /// </summary>
    public class RegisterDTO
    {
        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [MinLength(6)]
        [MaxLength(100)]
        public string Password { get; set; } = null!;

        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; } = null!;
    }
}