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

        public bool RegisterUser(string username, string password, string role, string securityId = "")
        {
            if (_context.Users.Any(u => u.Username == username))
            {
                return false; // Username already exists
            }

            var newUser = new User
            {
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


