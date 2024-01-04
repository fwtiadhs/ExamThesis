using System;
using System.Collections.Generic;

namespace ExamThesis.Storage.Model;

public partial class Question
{
    public int QuestionId { get; set; }

    public int QuestionCategoryId { get; set; }


    public int QuestionPoints { get; set; }

    public double NegativePoints { get; set; }

    public string QuestionText { get; set; } = null!;

    public virtual ICollection<Answer> Answers { get; set; } = new List<Answer>();

   
}
