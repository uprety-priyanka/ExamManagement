using System.ComponentModel.DataAnnotations;

namespace ExamManagement.Server.Entities
{
    public class ItemTypeCategory
    {
        [Key]
        public int Id { get; set; }
        public string CategoryName { get; set; }
        public string NormalizedCategoryName { get; set; }
    }
}
