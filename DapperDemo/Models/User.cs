using System.ComponentModel.DataAnnotations;

namespace DapperDemo.Models  
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Email { get; set; } = "";

        [Required]
        public string PasswordHash { get; set; } = "";
        public string Role { get; set; } = "Client";
    }
}
