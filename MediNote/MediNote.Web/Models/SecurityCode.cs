namespace MediNote.Web.Models
{
    public class SecurityCode
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty; // "Doctor" or "Admin"
        public bool IsClaimed { get; set; }
    }
}
