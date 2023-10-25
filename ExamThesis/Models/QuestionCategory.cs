using System.ComponentModel.DataAnnotations;

namespace ExamThesis.Models
{
    public class QuestionCategory
    {
        [Key]
        public int QuestionCategoryId { get; set; }
        [Required]
        public string QuestionCategoryName { get; set; }
    }
}
