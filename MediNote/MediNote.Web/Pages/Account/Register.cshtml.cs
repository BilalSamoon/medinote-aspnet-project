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
        public string Username { get; set; } = string.Empty;

        [BindProperty]
        public string Password { get; set; } = string.Empty;

        [BindProperty]
        public string Role { get; set; } = "Patient";

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
            {
                ModelState.AddModelError("", "Username and password are required.");
                return Page();
            }

            var success = _userRepository.RegisterUser(Username, Password, Role);

            if (!success)
            {
                ModelState.AddModelError("", "Username is already taken.");
                return Page();
            }

            return RedirectToPage("/Account/Login");
        }
    }
}