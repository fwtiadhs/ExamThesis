using System.ComponentModel.DataAnnotations;

namespace ExamThesis.Models
{
    public class QuestionCategory
    {
        [Key]
        public int CategoryId { get; set; }
        [Required]
        public string CategoryName { get; set; }
    }
}
