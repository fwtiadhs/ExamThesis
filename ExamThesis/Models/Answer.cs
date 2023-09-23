namespace ExamThesis.Models
{
    public class Answer
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Το πεδίο 'Κείμενο Απάντησης' είναι υποχρεωτικό.")]
        public string Text { get; set; }

        [Required(ErrorMessage = "Το πεδίο 'Σωστή Απάντηση' είναι υποχρεωτικό.")]
        public bool IsCorrect { get; set; }

        public int QuestionId { get; set; } // Εξωτερικό κλειδί προς την ερώτηση
    }
}
