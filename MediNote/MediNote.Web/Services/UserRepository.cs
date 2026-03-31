using System.Collections.Generic;
using System.Linq;
using MediNote.Web.Data;
using MediNote.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace MediNote.Web.Services
{
    public class UserRepository
    {
        private readonly MediNoteDbContext _context;

        public UserRepository(MediNoteDbContext context)
        {
            _context = context;
        }

        public User? Authenticate(string username, string password, string securityId = "")
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == username && u.Password == password);
            if (user != null)
            {
                if (user.Role == "Doctor" || user.Role == "Admin")
                {
                    if (user.SecurityId != securityId && !string.IsNullOrEmpty(user.SecurityId))
                    {
                        return null; // Invalid security ID
                    }
                }
            }
            return user;
        }

        public bool RegisterUser(string firstName, string lastName, string username, string password, string role, string securityId, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (_context.Users.Any(u => u.Username == username))
            {
                errorMessage = "Username already exists.";
                return false;
            }

            if (role == "Doctor" || role == "Admin")
            {
                var validCode = _context.SecurityCodes.FirstOrDefault(c => c.Code == securityId && c.Role == role && !c.IsClaimed);
                if (validCode == null)
                {
                    errorMessage = "Invalid or already claimed Security ID for the selected role.";
                    return false;
                }

                validCode.IsClaimed = true; // Mark as claimed
            }

            var newUser = new User
            {
                FirstName = firstName,
                LastName = lastName,
                Username = username,
                Password = password,
                Role = role,
                SecurityId = securityId
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();
            return true;
        }

        public void Migrate()
        {
            _context.Database.Migrate();
        }
    }
}


