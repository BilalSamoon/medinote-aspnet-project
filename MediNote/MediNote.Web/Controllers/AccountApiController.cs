using MediNote.Web.Contracts;
using MediNote.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace MediNote.Web.Controllers
{

    // By: Camila Esguerra
    // API controller for handling account-related operations such as registration.
    [ApiController]
    [Route("api/account")]
    public class AccountApiController : ControllerBase
    {
        private readonly UserRepository _userRepository;


        // Constructor that injects the UserRepository dependency.
        public AccountApiController(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // POST endpoint for registering a new user account.
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterAccountRequest request)
        {
            var success = _userRepository.RegisterUser(
                request.FirstName,
                request.LastName,
                request.Username,
                request.Password,
                request.Role,
                request.SecurityId,
                request.Email,
                out var errorMessage,
                out var issuedSecurityId,
                false);

            // If registration fails, return a BadRequest with the error message.
            if (!success)
            {
                return BadRequest(new { message = errorMessage });
            }

            var roleLabel = request.Role == "Doctor" || request.Role == "Admin" ? request.Role : "Patient";

            // If registration is successful, return an Ok response with a success message.
            return Ok(new
            {
                message = roleLabel == "Patient"
                    ? "Account created successfully."
                    : $"{roleLabel} account created successfully. You can now log in."
            });
        }
    }
}
