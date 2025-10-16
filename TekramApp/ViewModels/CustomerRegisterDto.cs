using System.ComponentModel.DataAnnotations;

namespace TekramApp.ViewModels
{
    public class CustomerRegisterDto
    {
        [Required]
        [StringLength(150, MinimumLength = 2)]
        public string Name { get; set; } = null!;

        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Username { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(100, MinimumLength = 8)]
        public string Password { get; set; } = null!;
    }
}
