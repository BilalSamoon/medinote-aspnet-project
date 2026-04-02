using MediNote.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MediNote.Web.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly UserRepository _userRepository;

        public RegisterModel(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [BindProperty]
        public string FirstName { get; set; } = string.Empty;

        [BindProperty]
        public string LastName { get; set; } = string.Empty;

        [BindProperty]
        public string Username { get; set; } = string.Empty;

        [BindProperty]
        public string Password { get; set; } = string.Empty;

        [BindProperty]
        public string ConfirmPassword { get; set; } = string.Empty;

        [BindProperty]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        public string Role { get; set; } = "Patient";

        [TempData]
        public string? SuccessMessage { get; set; }

        [TempData]
        public string? IssuedSecurityId { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName) || string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                ModelState.AddModelError(string.Empty, "First name, last name, username, and password are required.");
                return Page();
            }

            if (Password != ConfirmPassword)
            {
                ModelState.AddModelError(string.Empty, "Password and confirm password must match.");
                return Page();
            }

            var success = _userRepository.RegisterUser(FirstName, LastName, Username, Password, Role, string.Empty, Email, out string errorMessage, out string issuedSecurityId);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, errorMessage);
                return Page();
            }

            SuccessMessage = Role == "Doctor" || Role == "Admin"
                ? $"{Role} account created successfully. Please save your {Role} ID before logging in."
                : "Account created successfully. You can now log in.";
            IssuedSecurityId = issuedSecurityId;

            return RedirectToPage("/Account/Login");
        }
    }
}
