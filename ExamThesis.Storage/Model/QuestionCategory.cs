using System;
using System.Collections.Generic;

namespace ExamThesis.Storage.Model;

public partial class QuestionCategory
{
    public int QuestionCategoryId { get; set; }

    public string QuestionCategoryName { get; set; } = null!;

    public virtual ICollection<Exam> Exams { get; set; } = new List<Exam>();
}
