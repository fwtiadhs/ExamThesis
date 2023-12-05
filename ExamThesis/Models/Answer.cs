using System.ComponentModel.DataAnnotations;

namespace ExamThesis.Models
{
    public class Answer
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Το πεδίο 'Κείμενο Απάντησης' είναι υποχρεωτικό.")]
        public string Text { get; set; }

      
       public bool IsCorrect { get; set; }

       //public int QuestionId { get; set; } // Εξωτερικό κλειδί προς την ερώτηση
    }
}
