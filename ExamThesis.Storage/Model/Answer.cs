using System;
using System.Collections.Generic;

namespace ExamThesis.Storage.Model;

public partial class Answer
{
    public int Id { get; set; }

    public string Text { get; set; } = null!;

    public int? QuestionId { get; set; }
    public bool IsCorrect { get; set; }
    public virtual Question? Question { get; set; }
}
