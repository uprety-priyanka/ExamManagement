using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExamManagement.Server.Entities
{
    public class ItemType
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        [ForeignKey(nameof(ItemTypeCategory))]
        public int ItemTypeCategoryId { get; set; }
        public ItemTypeCategory ItemTypeCategory { get; set; }
    }
}
