namespace MediNote.Web.Models
{

    /// <summary>
    /// Author: Camila Esguerra
    /// User model for authentication and role management.
    /// </summary>
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string SecurityId { get; set; } = string.Empty; // This property exists in the model
    }
}

