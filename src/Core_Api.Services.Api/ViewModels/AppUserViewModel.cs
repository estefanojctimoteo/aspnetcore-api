using System;
using System.ComponentModel.DataAnnotations;
namespace Core_Api.Services.Api.ViewModels
{
    public class AppUserViewModel
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public bool Deleted { get; set; }

        public string Name { get; set; }
    }
}