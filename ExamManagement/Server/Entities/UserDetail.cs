using ExamManagement.Server.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExamManagement.Server.Entities
{
    public class UserDetail
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Faculty))]
        public int FacultyId { get; set; }
        public Faculty Faculty { get; set; }
        [ForeignKey(nameof(ApplicationUser))]
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}