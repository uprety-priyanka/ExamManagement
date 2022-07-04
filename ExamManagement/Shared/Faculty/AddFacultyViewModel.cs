using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamManagement.Shared.Faculty
{
    public class AddFacultyViewModel
    {
        [Required(ErrorMessage ="Faculty name is required.")]
        public string FacultyName { get; set; }
    }
}
