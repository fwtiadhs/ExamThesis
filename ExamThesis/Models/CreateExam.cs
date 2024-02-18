

namespace ExamThesis.Models
{
    public class CreateExam
    {
        public DateTime StartTime { get; set; } = DateTime.Now;

        public DateTime EndTime { get; set; } = DateTime.Now;

        public double TotalPoints { get; set; }

        public int ExamId { get; set; }

        public string? ExamName { get; set; }

        public List<QuestionCategory> SelectedCategories { get; set; }
        
    }
}
