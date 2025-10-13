using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace ExamThesis.Models
{
    public class QuestionCategory
    {
        public int QuestionCategoryId { get; set; }

        [Required(ErrorMessage = "Το όνομα κατηγορίας είναι υποχρεωτικό.")]
        public string QuestionCategoryName { get; set; } = string.Empty;
     
        public bool IsChecked { get; set; }
    }
}
