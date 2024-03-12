using System;
using System.Collections.Generic;

namespace ExamThesis.Storage.Model;

public partial class QuestionsInPackage
{
    public int? QuestionId { get; set; }

    public int? PackageId { get; set; }

    public int Id { get; set; }

    public virtual QuestionPackage? Package { get; set; }

    public virtual Question? Question { get; set; }
}
