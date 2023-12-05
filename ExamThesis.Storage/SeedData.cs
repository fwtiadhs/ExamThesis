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
            if (!context.QuestionTypes.Any())
            {
                await context.QuestionTypes.AddRangeAsync(GetQuestionTypes());
                await context.SaveChangesAsync();
            }
            if (!context.QuestionCategories.Any())
            {
                await context.QuestionCategories.AddRangeAsync(GetQuestionCategories());
                await context.SaveChangesAsync();
            }
            if (!context.Answers.Any())
            {
                await context.Answers.AddRangeAsync(GetAnswers());
                await context.SaveChangesAsync();
            }
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

        private static QuestionType[] GetQuestionTypes()
        {
            return new QuestionType[]
            {
                new QuestionType()
                {
                   // QuestionTypeId = 1,
                    QuestionTypeName = "type1"
                }
            };
        }
    }
}
