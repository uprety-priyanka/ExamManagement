using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExamManagement.Server.Entities
{
    public class UserDetailExtensionStudentTemporary
    {
        [Key]
        public int Id { get; set; }
        public int Semester { get; set; }
        public int ExamYear { get; set; }
        [ForeignKey(nameof(UserDetailExtension))]
        public int UserDetailExtensionId { get; set; }
        public UserDetailExtension UserDetailExtension { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
