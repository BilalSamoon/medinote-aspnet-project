namespace MediNote.Web.Models
{
    /// <summary>
    /// Application user used for login and portal access.
    /// </summary>
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string SecurityId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public string DisplayName => $"{FirstName} {LastName}".Trim();
    }
}
