using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExamManagement.Server.Entities
{
    public class ResultExtension
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Result))]
        public int ResultId { get; set; }
        public Result Result { get; set; }
        [ForeignKey(nameof(Course))]
        public int CourseId { get; set; }
        public Course Course { get; set; }
        public string Grade { get; set; }


    }
}
