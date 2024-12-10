namespace ProductInventoryManagerAPI.Models.Entities
{
    public class UserCredetials
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; } // Store the hashed password
        public string Role { get; set; }
    }
}
