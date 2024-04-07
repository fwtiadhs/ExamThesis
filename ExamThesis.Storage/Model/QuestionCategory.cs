using System;
using System.Collections.Generic;

namespace ExamThesis.Storage.Model;

public partial class QuestionCategory
{
    public int QuestionCategoryId { get; set; }

    public string QuestionCategoryName { get; set; } = null!;

    public virtual ICollection<ExamCategory> ExamCategories { get; set; } = new List<ExamCategory>();

    public virtual ICollection<QuestionPackage> QuestionPackages { get; set; } = new List<QuestionPackage>();
}
