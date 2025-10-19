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
           
            /*
            if (!context.QuestionCategories.Any())
            {
                await context.QuestionCategories.AddRangeAsync(GetQuestionCategories());
                await context.SaveChangesAsync();
            } 

            // ensure we have a category id to attach questions
            var categoryId = context.QuestionCategories.First().QuestionCategoryId;

            if (!context.Questions.Any())
            {
                await context.Questions.AddRangeAsync(GetQuestions(categoryId));
                await context.SaveChangesAsync();
            }

            // ensure we have a question id to attach answers
            var questionId = context.Questions.First().QuestionId;

            if (!context.Answers.Any())
            {
                await context.Answers.AddRangeAsync(GetAnswers(questionId));
                await context.SaveChangesAsync();
            }
            */
        }

        private static Question[] GetQuestions(int categoryId)
        {

            return new Question[]
            {
                new Question
                {
                    QuestionPoints = 0,
                    NegativePoints = 0,
                    QuestionText = "test",
                    QuestionCategoryId = categoryId
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
        private static Answer[] GetAnswers(int questionId)
        {
            return new Answer[]
            {
                new Answer()
                {
                    Text = "Text",
                    QuestionId = questionId
                }
            };
        }

       
    }
}
