using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamManagement.Shared.Account
{
    public class AddStudentViewModel
    {
        [Required(ErrorMessage ="First name is required.")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Last name is required.")]
        public string LastName { get; set; }
        [Required(ErrorMessage ="Username is required.")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Email address is required.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Registration number is required.")]
        public string RegistrationNumber { get; set; }
        [Required(ErrorMessage = "Exam number is required.")]
        public string ExamNumber { get; set; }
        [Required(ErrorMessage = "Exam year is required.")]
        public int ExamYear { get; set; }
        [Required(ErrorMessage = "Batch year is required.")]
        public int Batch { get; set; }
        [Required(ErrorMessage = "Roll number is required.")]
        public int RollNumber { get; set; }
        [Required]
        public int FacultyId { get; set; }
        [Required(ErrorMessage ="Password is required.")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Confirm password is required."), Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }


    }
}
