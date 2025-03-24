using System.ComponentModel.DataAnnotations;

namespace RiceRunner.Models
{
    public class InventoryItem
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [MaxLength(50)]
        public string? ItemName { get; set; }

        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }

        public User User { get; set; }
    }
}
