using System;
using System.ComponentModel.DataAnnotations;
namespace JetStreamAPI.Models
{


    public class Employee
    {
        [Key]
        public int EmployeeId { get; set; }

        [Required]
        [StringLength(100)]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        public bool IsLocked { get; set; } = false;
        public int FailedLoginAttempts { get; set; } = 0;

        // Method to reset attempts
        public void ResetLock()
        {
            IsLocked = false;
            FailedLoginAttempts = 0;
        }

        // Method to check if its failed
        public void ProcessFailedLogin()
        {
            FailedLoginAttempts++;
            if (FailedLoginAttempts >= 3)
            {
                IsLocked = true;
            }
        }

    }
}
