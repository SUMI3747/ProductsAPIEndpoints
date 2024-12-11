using System.ComponentModel.DataAnnotations;

namespace ProductInventoryManagerAPI.Models.DTOs
{
    public class LoginDto
    {
        public int UserId { get; set; }

        public string? Password { get; set; }   
    }
}
