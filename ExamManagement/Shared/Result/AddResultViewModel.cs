using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamManagement.Shared.Result
{
    public class AddResultViewModel
    {
        public int CourseId {get; set;}
        [Required(ErrorMessage = "Grade is required to add a result.")]
        public string Grade { get; set; }
        public int UserDetailExxtensionTemporartStudentId { get; set; }
    }
}
