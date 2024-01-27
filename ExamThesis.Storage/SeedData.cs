using ExamThesis.Storage.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamThesis.Storage
{
   public static  class SeedData
    {
        

        public static async Task Seed(ExamContext context)
        {
           
            if (!context.QuestionCategories.Any())
            {
                await context.QuestionCategories.AddRangeAsync(GetQuestionCategories());
                await context.SaveChangesAsync();
            } 
            if (!context.Questions.Any())
            {
                await context.Questions.AddRangeAsync(GetQuestions());
                await context.SaveChangesAsync();
            }
            if (!context.Answers.Any())
            {
                await context.Answers.AddRangeAsync(GetAnswers());
                await context.SaveChangesAsync();
            }
        }

        private static Question[] GetQuestions()
        {

            return new Question[]
            {
                new Question
                {
                    QuestionPoints = 0,
                    NegativePoints = 0,
                    QuestionText = "test"


                }
            };
        }

        private static QuestionCategory[] GetQuestionCategories()
        {
            return new QuestionCategory[]
            {
                new QuestionCategory()
                {
                    QuestionCategoryName = "category1"
                }
            };
        }
        private static Answer[] GetAnswers()
        {
            return new Answer[]
            {
                new Answer()
                {
                    Text = "Text",
                    QuestionId = 1
                }
            };
        }

       
    }
}
