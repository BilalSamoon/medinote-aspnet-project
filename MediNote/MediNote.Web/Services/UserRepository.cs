using System.Collections.Generic;
using System.Linq;
using MediNote.Web.Data;
using MediNote.Web.Models;

namespace MediNote.Web.Services
{
    public class UserRepository
    {
        private readonly MediNoteDbContext _context;

        public UserRepository(MediNoteDbContext context)
        {
            _context = context;
        }

        public User? Authenticate(string username, string password)
        {
            return _context.Users.FirstOrDefault(u => u.Username == username && u.Password == password);
        }

        public bool RegisterUser(string username, string password, string role)
        {
            if (_context.Users.Any(u => u.Username == username))
            {
                return false; // Username already exists
            }

            var newUser = new User
            {
                Username = username,
                Password = password,
                Role = role
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();
            return true;
        }
    }
}
