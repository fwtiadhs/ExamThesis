using System;
using System.Collections.Generic;

namespace ExamThesis.Storage.Model;

public partial class QuestionCategory
{
    public int QuestionCategoryId { get; set; }

    public string QuestionCategoryName { get; set; } = null!;

    public byte[]? FileData { get; set; }

    public virtual ICollection<ExamCategory> ExamCategories { get; set; } = new List<ExamCategory>();
}
