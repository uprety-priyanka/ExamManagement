using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExamManagement.Server.Entities
{
    public class ExamForm
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey(nameof(UserDetailExtensionStudentTemporary))]
        public int UserDetailExtensionStudentTemporaryId { get; set; }
        public UserDetailExtensionStudentTemporary UserDetailExtensionStudentTemporary { get; set; }

        public DateTime FormFilledDate { get; set; }
    }
}
