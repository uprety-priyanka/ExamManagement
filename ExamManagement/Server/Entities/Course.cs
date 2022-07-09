using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExamManagement.Server.Entities
{
    public class Course
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey(nameof(Faculty))]
        public int FacultyId { get; set; }
        public Faculty Faculty { get; set; }
        [ForeignKey(nameof(SemesterType))]
        public int SemesterTypeId { get; set; }
        public ItemType SemesterType { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public string ConcurrentRegistrationCourse { get; set; }
        public string PreRequisiteCourse { get; set; }
        public int Credit { get; set; }
        public int Lecture { get; set; }
        public int Tutorial { get; set; }
        public int Practical { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
