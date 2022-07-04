using System.ComponentModel.DataAnnotations;

namespace ExamManagement.Server.Entities
{
    public class Faculty
    {
        [Key]
        public int Id { get; set; }
        public string FacultyName { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
