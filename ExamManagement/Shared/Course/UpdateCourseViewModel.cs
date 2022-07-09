using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamManagement.Shared.Course
{
    public class UpdateCourseViewModel
    {
        public int Id { get; set; }
        [Required]
        public int FacultyId { get; set; }
        [Required]
        public int SemesterTypeId { get; set; }
        [Required(ErrorMessage = "Course code is required.")]
        public string CourseCode { get; set; }
        [Required(ErrorMessage = "Course name is required.")]
        public string CourseName { get; set; }
        public string ConcurrentRegistrationCourse { get; set; }
        public string PreRequisiteCourse { get; set; }
        [Required(ErrorMessage = "Credit is required.")]
        public int Credit { get; set; }
        public int Lecture { get; set; }
        public int Tutorial { get; set; }
        public int Practical { get; set; }
    }
}
