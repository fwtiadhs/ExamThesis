using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamThesis.Storage.Model
{
    public class CreateQuestion
    {
        public string QuestionText { get; set; }

        public List<CreateAnswer> Answers { get; set; }
    }
    public class CreateAnswer
    {
        public string Text { get; set; }

        public bool IsCorrect { get; set; }
    }
}
