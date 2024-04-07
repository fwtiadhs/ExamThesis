using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ExamThesis.Models
{
    public class QuestionCategory
    {
        public int QuestionCategoryId { get; set; }

        [ValidateNever]
        public string QuestionCategoryName { get; set; }
     
        public bool IsChecked { get; set; }
    }
}
