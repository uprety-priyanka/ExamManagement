using System.ComponentModel.DataAnnotations;

using System.ComponentModel.DataAnnotations.Schema;

namespace ExamManagement.Server.Entities
{
    public class UserDetailExtension
    {
        [Key]
        public int Id { get; set; }
        public string RegistrationNumber { get; set; }
        public string ExamNumber { get; set; }

        [ForeignKey(nameof(UserDetail))]
        public int UserDetailId { get; set; }
        public UserDetail UserDetail { get; set; }
        public DateTime CreatedDate { get; set; }

    }
}
