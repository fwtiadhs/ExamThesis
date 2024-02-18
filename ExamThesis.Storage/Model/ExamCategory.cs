using System;
using System.Collections.Generic;

namespace ExamThesis.Storage.Model;

public partial class ExamCategory
{
    public int ExamId { get; set; }

    public int QuestionCategoryId { get; set; }

    public virtual Exam Exam { get; set; } = null!;

    public virtual QuestionCategory QuestionCategory { get; set; } = null!;
}
