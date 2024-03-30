using ExamThesis.Storage.Model;

namespace ExamThesis.Models
{
    public class ExamViewModel
    {
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public double TotalPoints { get; set; }
        public int ExamId { get; set; }
        public string ExamName { get; set; }
        public double PassGrade { get; set; }

        // Πεδία από το μοντέλο Exam
        public ICollection<ExamCategory> ExamCategories { get; set; }
        public ICollection<ExamResult> ExamResults { get; set; }

        // Πεδία από το μοντέλο Question
        public int QuestionId { get; set; }
        public int QuestionCategoryId { get; set; }
        public double NegativePoints { get; set; }
        public string QuestionText { get; set; }
        public double QuestionPoints { get; set; }
        public int? PackageId { get; set; }
        public ICollection<Answer> Answers { get; set; }
        public QuestionPackage Package { get; set; }
        public ICollection<QuestionsInPackage> QuestionsInPackages { get; set; }
    }
}
