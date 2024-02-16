using System;
using System.Collections.Generic;

namespace ExamThesis.Storage.Model;

public partial class Exam
{
    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public double TotalPoints { get; set; }

    public int? QuestionCategoryId { get; set; }

    public int ExamId { get; set; }

    public string? ExamName { get; set; }

    public virtual QuestionCategory? QuestionCategory { get; set; }

    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
}
