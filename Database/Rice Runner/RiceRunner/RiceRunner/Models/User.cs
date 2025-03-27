using System.ComponentModel.DataAnnotations;

namespace RiceRunner.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string? Username { get; set; }

        [Required]
        [MaxLength(100)]
        public string? Password { get; set; }

        public string? Email { get; set; }

        public int Score { get; set; }
        public int Rice { get; set; }

    }
}
