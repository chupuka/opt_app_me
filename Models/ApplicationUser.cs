using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace ProForm.Models
{
    public class ApplicationUser : IdentityUser
    {
        [StringLength(255)]
        public string? FullName { get; set; }
    }
}

