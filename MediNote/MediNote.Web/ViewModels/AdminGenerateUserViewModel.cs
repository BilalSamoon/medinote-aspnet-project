using System.ComponentModel.DataAnnotations;

namespace MediNote.Web.ViewModels
{
    //By: Camila Esguerra
    // ViewModel for the admin user generation page, containing properties for user details and validation attributes to ensure required fields are filled out correctly.
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