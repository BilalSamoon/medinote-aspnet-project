using System.Collections.Generic;
using System.Linq;
using MediNote.Web.Models;

namespace MediNote.Web.Services
{
    public class UserRepository
    {
        private readonly List<User> _users = new List<User>();

        public UserRepository()
        {
            // Hardcoded initial users for demonstration purposes
            _users.Add(new User { Id = 1, Username = "doctor1", Password = "password123", Role = "Doctor" });
            _users.Add(new User { Id = 2, Username = "admin1", Password = "adminpassword", Role = "Admin" });
            _users.Add(new User { Id = 3, Username = "patient1", Password = "patientpassword", Role = "Patient" });
        }

        public User? Authenticate(string username, string password)
        {
            return _users.FirstOrDefault(u => u.Username == username && u.Password == password);
        }

        public bool RegisterUser(string username, string password, string role)
        {
            if (_users.Any(u => u.Username == username))
            {
                return false; // Username already exists
            }

            var newUser = new User
            {
                Id = _users.Count > 0 ? _users.Max(u => u.Id) + 1 : 1,
                Username = username,
                Password = password,
                Role = role
            };

            _users.Add(newUser);
            return true;
        }
    }
}
