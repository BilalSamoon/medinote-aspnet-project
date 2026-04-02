using MediNote.Web.Contracts;
using MediNote.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace MediNote.Web.Controllers
{
    [ApiController]
    [Route("api/account")]
    public class AccountApiController : ControllerBase
    {
        private readonly UserRepository _userRepository;

        public AccountApiController(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

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
                out var issuedSecurityId);

            if (!success)
            {
                return BadRequest(new { message = errorMessage });
            }

            var roleLabel = request.Role == "Doctor" || request.Role == "Admin" ? request.Role : "Patient";

            return Ok(new
            {
                message = roleLabel == "Patient"
                    ? "Account created successfully."
                    : $"{roleLabel} account created successfully. Save the issued {roleLabel} ID for future login.",
                issuedSecurityId
            });
        }
    }
}
