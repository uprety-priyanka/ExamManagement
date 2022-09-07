using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExamManagement.Server.Entities
{
    public class ExamFormRegular
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey(nameof(Course))]
        public int CourseId { get; set; }
        public Course Course { get; set; }
        [ForeignKey(nameof(ExamForm))]
        public int ExamFormId { get; set; }
        public ExamForm ExamForm { get; set; }
    }
}
