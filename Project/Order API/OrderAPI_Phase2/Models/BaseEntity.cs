using System.ComponentModel.DataAnnotations;

namespace OrderAPI_Phase2.Models
{
    public abstract class BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string CreatedBy { get; set; } = string.Empty;

        public string? UpdatedBy { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
