using System;
using System.Collections.Generic;
using System.Linq;
using MediNote.Web.Data;
using MediNote.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace MediNote.Web.Services
{
    /// <summary>
    /// Repository responsible for authentication and registration.
    /// </summary>
    public class UserRepository
    {
        private readonly MediNoteDbContext _context;

        public UserRepository(MediNoteDbContext context)
        {
            _context = context;
        }

        public string GenerateSecurityCodeForRole(string role)
        {
            var normalizedRole = string.IsNullOrWhiteSpace(role) ? "Doctor" : role.Trim();
            var prefix = string.Equals(normalizedRole, "Admin", StringComparison.OrdinalIgnoreCase) ? "ADM" : "DOC";
            var code = GenerateRoleId(prefix);

            _context.SecurityCodes.Add(new SecurityCode
            {
                Code = code,
                Role = normalizedRole,
                IsClaimed = false
            });
            _context.SaveChanges();

            return code;
        }

        public User? Authenticate(string username, string password, string securityId = "")
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == username && u.Password == password);
            if (user == null)
            {
                return null;
            }

            if ((user.Role == "Doctor" || user.Role == "Admin") && !string.IsNullOrEmpty(user.SecurityId))
            {
                if (!string.Equals(user.SecurityId, securityId?.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    return null;
                }
            }

            return user;
        }

        public bool RegisterUser(
            string firstName,
            string lastName,
            string username,
            string password,
            string role,
            string securityId,
            string email,
            out string errorMessage,
            out string issuedSecurityId,
            bool isAdminAction = false)
        {
            errorMessage = string.Empty;
            issuedSecurityId = string.Empty;

            var normalizedRole = string.IsNullOrWhiteSpace(role) ? "Patient" : role.Trim();
            var normalizedUsername = username?.Trim() ?? string.Empty;
            var normalizedEmail = string.IsNullOrWhiteSpace(email)
                ? $"{normalizedUsername}@medinote.local"
                : email.Trim();

            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) || string.IsNullOrWhiteSpace(normalizedUsername) || string.IsNullOrWhiteSpace(password))
            {
                errorMessage = "First name, last name, username, and password are required.";
                return false;
            }

            if (_context.Users.Any(u => u.Username == normalizedUsername))
            {
                errorMessage = "Username already exists.";
                return false;
            }

            var resolvedSecurityId = string.Empty;
            if (string.Equals(normalizedRole, "Doctor", StringComparison.OrdinalIgnoreCase) || string.Equals(normalizedRole, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                if (isAdminAction)
                {
                    var prefix = string.Equals(normalizedRole, "Admin", StringComparison.OrdinalIgnoreCase) ? "ADM" : "DOC";
                    resolvedSecurityId = GenerateRoleId(prefix);
                    issuedSecurityId = resolvedSecurityId;
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(securityId))
                    {
                        errorMessage = $"{normalizedRole} registration requires a valid Security ID provided by an administrator.";
                        return false;
                    }

                    var codeRecord = _context.SecurityCodes.FirstOrDefault(c => c.Code == securityId && c.Role == normalizedRole);
                    if (codeRecord == null || codeRecord.IsClaimed)
                    {
                        errorMessage = $"The provided {normalizedRole} Security ID is invalid or already claimed.";
                        return false;
                    }

                    resolvedSecurityId = securityId;
                }
            }

            var newUser = new User
            {
                FirstName = firstName.Trim(),
                LastName = lastName.Trim(),
                Username = normalizedUsername,
                Password = password,
                Role = normalizedRole,
                SecurityId = resolvedSecurityId,
                Email = normalizedEmail
            };

            _context.Users.Add(newUser);

            if (!string.IsNullOrWhiteSpace(resolvedSecurityId))
            {
                var existingCode = _context.SecurityCodes.FirstOrDefault(c => c.Code == resolvedSecurityId);
                if (existingCode == null)
                {
                    _context.SecurityCodes.Add(new SecurityCode
                    {
                        Code = resolvedSecurityId,
                        Role = normalizedRole,
                        IsClaimed = true
                    });
                }
                else
                {
                    existingCode.Role = normalizedRole;
                    existingCode.IsClaimed = true;
                    _context.SecurityCodes.Update(existingCode);
                }
            }

            _context.SaveChanges();
            return true;
        }

        public bool RegisterUser(string firstName, string lastName, string username, string password, string role, string securityId, out string errorMessage)
        {
            return RegisterUser(firstName, lastName, username, password, role, securityId, string.Empty, out errorMessage, out _, false);
        }

        public List<User> GetDoctors()
        {
            return _context.Users
                .Where(u => u.Role == "Doctor")
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName)
                .ToList();
        }

        public List<User> GetAdmins()
        {
            return _context.Users
                .Where(u => u.Role == "Admin")
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName)
                .ToList();
        }

        public User? GetUserByDisplayName(string displayName)
        {
            return _context.Users.FirstOrDefault(u => (u.FirstName + " " + u.LastName).Trim() == displayName);
        }

        public User? GetUserByUsername(string username)
        {
            return _context.Users.FirstOrDefault(u => u.Username == username);
        }

        public void Migrate()
        {
            _context.Database.Migrate();
        }

        private string GenerateRoleId(string prefix)
        {
            string candidate;
            do
            {
                candidate = $"{prefix}-{Random.Shared.Next(100000, 999999)}";
            }
            while (_context.Users.Any(u => u.SecurityId == candidate) || _context.SecurityCodes.Any(c => c.Code == candidate));

            return candidate;
        }
    }
}
