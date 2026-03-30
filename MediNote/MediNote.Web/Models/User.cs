namespace MediNote.Web.Models
{

    /// <summary>
    /// Author: Camila Esguerra
    /// User model for authentication and role management.
    /// </summary>
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty; // Store plain text for now, should be hashed later
        public string Role { get; set; } = string.Empty; // "Admin", "Doctor", "Patient"
    }
}
