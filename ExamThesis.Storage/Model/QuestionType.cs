using System;
using System.Collections.Generic;

namespace ExamThesis.Storage.Model;

public partial class QuestionType
{
    public int QuestionTypeId { get; set; }

    public string QuestionTypeName { get; set; } = null!;

    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
}
