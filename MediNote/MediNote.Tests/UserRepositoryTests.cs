using System.Linq;
using MediNote.Web.Data;
using MediNote.Web.Models;
using MediNote.Web.Services;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace MediNote.Tests
{
    // By: Camila Esguerra
    // Comprehensive unit tests for UserRepository covering registration and authentication scenarios, including security code handling.
    public class UserRepositoryTests
    {
        private MediNoteDbContext _context = null!;
        private UserRepository _userRepository = null!;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<MediNoteDbContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;

            _context = new MediNoteDbContext(options);
            _userRepository = new UserRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public void RegisterUser_Patient_Success_NoSecurityIdRequired()
        {
            // Act
            var success = _userRepository.RegisterUser(
                "John", "Doe", "jdoe", "pass123", "Patient", "", "jdoe@test.local", out var errorMessage, out var issuedId, false);

            // Assert
            Assert.That(success, Is.True);
            Assert.That(errorMessage, Is.Empty);
            
            var user = _context.Users.FirstOrDefault(u => u.Username == "jdoe");
            Assert.That(user, Is.Not.Null);
            Assert.That(user.Role, Is.EqualTo("Patient"));
        }

        [Test]
        public void RegisterUser_Doctor_Fails_WithoutSecurityId()
        {
            // Act
            var success = _userRepository.RegisterUser(
                "Jane", "Smith", "jsmith", "pass123", "Doctor", "", "jsmith@test.local", out var errorMessage, out var issuedId, false);

            // Assert
            Assert.That(success, Is.False);
            Assert.That(errorMessage, Does.Contain("requires a valid Security ID"));
            Assert.That(_context.Users.Any(u => u.Username == "jsmith"), Is.False);
        }

        [Test]
        public void RegisterUser_Admin_Success_WithValidSecurityId()
        {
            // Arrange
            var code = "ADM-123456";
            _context.SecurityCodes.Add(new SecurityCode { Code = code, Role = "Admin", IsClaimed = false });
            _context.SaveChanges();

            // Act
            var success = _userRepository.RegisterUser(
                "Admin", "User", "adminuser", "pass123", "Admin", code, "admin@test.local", out var errorMessage, out var issuedId, false);

            // Assert
            Assert.That(success, Is.True);
            Assert.That(errorMessage, Is.Empty);

            var user = _context.Users.FirstOrDefault(u => u.Username == "adminuser");
            Assert.That(user, Is.Not.Null);
            Assert.That(user.SecurityId, Is.EqualTo(code));

            var dbCode = _context.SecurityCodes.First(c => c.Code == code);
            Assert.That(dbCode.IsClaimed, Is.True);
        }

        [Test]
        public void RegisterUser_IsAdminAction_GeneratesSecurityIdAndRegistersUser()
        {
            // Act
            var success = _userRepository.RegisterUser(
                "New", "Doc", "newdoc", "pass123", "Doctor", "", "newdoc@test.local", out var errorMessage, out var issuedId, true);

            // Assert
            Assert.That(success, Is.True);
            Assert.That(issuedId, Is.Not.Empty);

            var user = _context.Users.FirstOrDefault(u => u.Username == "newdoc");
            Assert.That(user, Is.Not.Null);
            Assert.That(user.SecurityId, Is.EqualTo(issuedId));

            var dbCode = _context.SecurityCodes.FirstOrDefault(c => c.Code == issuedId);
            Assert.That(dbCode, Is.Not.Null);
            Assert.That(dbCode.IsClaimed, Is.True);
        }

        [Test]
        public void GenerateSecurityCodeForRole_Success()
        {
            // Act
            var code = _userRepository.GenerateSecurityCodeForRole("Doctor");

            // Assert
            Assert.That(code, Is.Not.Empty);
            Assert.That(code, Does.StartWith("DOC-"));

            var dbCode = _context.SecurityCodes.FirstOrDefault(c => c.Code == code);
            Assert.That(dbCode, Is.Not.Null);
            Assert.That(dbCode.Role, Is.EqualTo("Doctor"));
            Assert.That(dbCode.IsClaimed, Is.False);
        }

        [Test]
        public void Authenticate_ValidDoctorWithCorrectSecurityId_ReturnsUser()
        {
            // Arrange
            var user = new User { FirstName = "Dr", LastName = "Whim", Username = "drwhim", Password = "pw", Role = "Doctor", SecurityId = "DOC-111222" };
            _context.Users.Add(user);
            _context.SaveChanges();

            // Act
            var result = _userRepository.Authenticate("drwhim", "pw", "DOC-111222");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Username, Is.EqualTo("drwhim"));
        }

        [Test]
        public void Authenticate_DoctorWithInvalidSecurityId_ReturnsNull()
        {
            // Arrange
            var user = new User { FirstName = "Dr", LastName = "Whim", Username = "drwhim", Password = "pw", Role = "Doctor", SecurityId = "DOC-111222" };
            _context.Users.Add(user);
            _context.SaveChanges();

            // Act
            var result = _userRepository.Authenticate("drwhim", "pw", "WRONG_ID");

            // Assert
            Assert.That(result, Is.Null);
        }
    }
}