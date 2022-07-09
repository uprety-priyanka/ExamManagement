using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamManagement.Shared.Faculty
{
    public class UpdateFacultyViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Faculty name is required.")]
        public string FacultyName { get; set; }
    }
}
