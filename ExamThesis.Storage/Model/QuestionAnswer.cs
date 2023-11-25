using System;
using System.Collections.Generic;

namespace ExamThesis.Storage.Model;

public partial class QuestionAnswer
{
    public int QuestionId { get; set; }

    public int AnswerId { get; set; }

    public bool IsCorrect { get; set; }

    public virtual Answer Answer { get; set; } = null!;

    public virtual Question Question { get; set; } = null!;
}
