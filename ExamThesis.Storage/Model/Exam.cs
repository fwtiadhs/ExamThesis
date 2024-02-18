using System;
using System.Collections.Generic;

namespace ExamThesis.Storage.Model;

public partial class Exam
{
    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public double TotalPoints { get; set; }

    public int ExamId { get; set; }

    public string? ExamName { get; set; }
}
