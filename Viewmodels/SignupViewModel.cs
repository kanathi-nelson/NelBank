using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NelBank.Viewmodels
{
    public class SignupViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
         [Required]
        [Display(Name = "Middle Name")]
        public string MiddleName { get; set; }
         [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        public string Id_Number { get; set; }
        public string AccountNumber { get; set; }

        [Required]
        public DateTime DateofBirth { get; set; }

        // [Required]
        [Display(Name = "Role")]
        public int SelectedRole { get; set; }
        // public IEnumerable<SelectListItem> Roles { get; set; }
        
        public string Gender { get; set; }

        [Display(Name = "Address")]
        public string Address { get; set; }     

        
        [EmailAddress]
        [Display(Name = "Alternative Email")]
        public string AlternativeEmail { get; set; }

        [Display(Name = "Phone Number")]
        [MaxLength(15, ErrorMessage = "Phone Number cannot be more than 15 digits")]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }


    }
}
