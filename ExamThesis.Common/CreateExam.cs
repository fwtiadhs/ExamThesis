
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamThesis.Common
{
    public class CreateExam
    {
        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public double TotalPoints { get; set; }

        public int ExamId { get; set; }

        public string? ExamName { get; set; }

        public List<QuestionCategory> SelectedCategories { get; set; }

    }
}
