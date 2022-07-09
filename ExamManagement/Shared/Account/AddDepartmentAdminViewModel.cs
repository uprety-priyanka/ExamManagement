using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamManagement.Shared.Account
{
    public class AddDepartmentAdminViewModel
    {
        [Required(ErrorMessage ="First Name is required.")]
        public string GivenName { get; set; }
        [Required(ErrorMessage = "Last Name is required.")]
        public string SurName { get; set; }
        [Required(ErrorMessage = "Email address is required.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Username is required.")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }
        [Compare(nameof(Password), ErrorMessage ="Password and confirm password should match.")]
        public string ConfirmPassword { get; set; }

        public int FacultyId { get; set; }
    }
}
