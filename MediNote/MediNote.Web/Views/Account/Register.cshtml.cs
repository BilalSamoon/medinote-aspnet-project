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
        public string Role { get; set; } = "Patient";

        [BindProperty]
        public string SecurityId { get; set; } = string.Empty;

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (string.IsNullOrEmpty(FirstName) || string.IsNullOrEmpty(LastName) || string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
            {
                ModelState.AddModelError("", "First Name, Last Name, Username, and Password are required.");
                return Page();
            }

            if ((Role == "Doctor" || Role == "Admin") && string.IsNullOrEmpty(SecurityId))
            {
                ModelState.AddModelError("", "Doctor/Admin ID is required for this role.");
                return Page();
            }

            var success = _userRepository.RegisterUser(FirstName, LastName, Username, Password, Role, SecurityId, out string errorMessage);
            if (!success)
            {
                ModelState.AddModelError("", errorMessage);
                return Page();
            }

            return RedirectToPage("/Account/Login");
        }
    }
}