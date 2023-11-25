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
