using System;
using System.Collections.Generic;

namespace ExamThesis.Storage.Model;

public partial class Question
{
    public int QuestionId { get; set; }

    public int QuestionCategoryId { get; set; }

    public double NegativePoints { get; set; }

    public string QuestionText { get; set; } = null!;

    public double QuestionPoints { get; set; }

    public int? PackageId { get; set; }

    public byte[]? ImageData { get; set; }

    public string? ImageType { get; set; }

    public virtual ICollection<Answer> Answers { get; set; } = new List<Answer>();

    public virtual QuestionPackage? Package { get; set; }

    public virtual ICollection<QuestionsInPackage> QuestionsInPackages { get; set; } = new List<QuestionsInPackage>();
}
