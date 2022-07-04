using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamManagement.Shared.Account
{
    public class RegisterDepartmentAdminViewModel
    {
        [Required(ErrorMessage = "First Name is required.")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Last Name is required.")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Username is required.")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Email address is required.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }
        [Compare(nameof(Password), ErrorMessage = "Password and Confirm Password should match.")]
        public string ConfirmPassword { get; set; }
        [Required(ErrorMessage = "Faculty is required.")]
        public int FacultyId { get; set; }
    }
}
