namespace MediNote.Web.Models
{
    //By: Camila Esguerra
    // This class represents a security code that can be used by doctors and admins to register for the application. Each code has an associated role (either "Doctor" or "Admin") and a flag indicating whether it has been claimed or not. The Id property serves as the primary key for the database, while the Code property stores the actual security code string.
    public class SecurityCode
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty; // "Doctor" or "Admin"
        public bool IsClaimed { get; set; }
    }
}
