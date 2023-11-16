using System;
using JetStreamAPI;
using System.ComponentModel.DataAnnotations;

namespace JetStreamAPI.Models
{

    public class ServiceOrder
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string CustomerName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        public string Priority { get; set; }

        [Required]
        public string ServiceType { get; set; }

        [Required]
        public DateTime CreationDate { get; set; }

        [Required]
        public DateTime PickupDate { get; set; }

        // for later configuration
        public string Comments { get; set; } = "";

        public string Status { get; set; } = "Offen";
    }
}