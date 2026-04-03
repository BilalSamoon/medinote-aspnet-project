using System.ComponentModel.DataAnnotations;

namespace MediNote.Web.ViewModels
{
    public class AdminGenerateUserViewModel
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = "Doctor";

        public string SuccessMessage { get; set; } = string.Empty;
        public string IssuedSecurityId { get; set; } = string.Empty;
    }
}