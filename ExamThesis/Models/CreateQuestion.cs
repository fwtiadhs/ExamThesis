using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.WebPages.Html;


namespace ExamThesis.Models
{
    public class CreateQuestion
    {
        

        public CreateQuestion()
        {
            Answers = new List<CreateAnswer>();
            Answers.Add(new CreateAnswer() { });
            Answers.Add(new CreateAnswer() { });
        }
        public string QuestionText { get; set; }

        public List<CreateAnswer> Answers { get; set; }
        public double QuestionPoints { get;  set; }
        public double NegativePoints { get;  set; }
    }
    public class CreateAnswer
    {
        public string Text { get; set; }

        public bool IsCorrect { get; set; }
    }
}
