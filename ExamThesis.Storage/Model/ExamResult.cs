using System;
using System.Collections.Generic;

namespace ExamThesis.Storage.Model;

public partial class ExamResult
{
    public int ExamResultId { get; set; }

    public int ExamId { get; set; }

    public double Grade { get; set; }

    public virtual Exam Exam { get; set; } = null!;
}
