using JetStreamAPI.Models;
using System.ComponentModel.DataAnnotations;

public class ServiceType
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    [Required]
    public decimal Cost { get; set; }
}
