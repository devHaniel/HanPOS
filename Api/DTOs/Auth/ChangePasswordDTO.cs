using System.ComponentModel.DataAnnotations;

namespace Api.DTOs.Auth
{
    /// <summary>
    /// Data Transfer Object for changing password.
    /// </summary>
    public class ChangePasswordDTO
    {
        [Required]
        public string CurrentPassword { get; set; } = null!;

        [Required]
        [MinLength(6)]
        [MaxLength(100)]
        public string NewPassword { get; set; } = null!;

        [Required]
        [Compare("NewPassword")]
        public string ConfirmNewPassword { get; set; } = null!;
    }
}