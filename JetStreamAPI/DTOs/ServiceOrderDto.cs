using System.ComponentModel.DataAnnotations;

namespace JetStreamAPI.DTOs
{
    public class ServiceOrderDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Phone]
        public string Phone { get; set; }

        [Required]
        public string Priority { get; set; }

        [Required]
        public int ServiceTypeId { get; set; }

        [Required]
        public DateTime CreationDate { get; set; }

        [Required]
        public DateTime PickupDate { get; set; }

        public string Comment { get; set; }
        public string Status { get; set; }
    }
}
