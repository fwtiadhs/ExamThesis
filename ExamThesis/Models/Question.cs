using System.ComponentModel.DataAnnotations;

namespace ExamThesis.Models
{
    public class Question
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; }
        public int CategoryId { get; set; }
        public List<Answer> Answers { get; set; }
        public int CorrectAnswerId { get; set; }
        public byte[]? ImageData { get; set; } // Τα δεδομένα της εικόνας (nullable)
        public string? ImageMimeType { get; set; } // Ο τύπος της εικόνας (π.χ., image/jpeg, nullable)
        public int Points { get; set; } // Τα βαθμολογικά σημεία για την ερώτηση
        public QuestionType Type { get; set; } // Το είδος της ερώτησης (πολλαπλής επιλογής ή ανάπτυξης)
    }

}
