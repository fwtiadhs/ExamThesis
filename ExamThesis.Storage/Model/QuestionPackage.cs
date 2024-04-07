using System;
using System.Collections.Generic;

namespace ExamThesis.Storage.Model;

public partial class QuestionPackage
{
    public int PackageId { get; set; }

    public string? PackageName { get; set; }

    public int? QuestionCategoryId { get; set; }

    public byte[]? FileData { get; set; }

    public virtual QuestionCategory? QuestionCategory { get; set; }

    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();

    public virtual ICollection<QuestionsInPackage> QuestionsInPackages { get; set; } = new List<QuestionsInPackage>();
}
