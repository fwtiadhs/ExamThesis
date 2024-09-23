using ExamThesis.Storage.Model;
using System;
using System.Collections.Generic;

namespace ExamThesis.Storage
{
    public class ExamQuestionViewModel
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public double TotalPoints { get; set; }
        public int ExamId { get; set; }
        public string ExamName { get; set; }
        public double PassGrade { get; set; }
        public bool? ShowGrade { get; set; }

        public int QuestionId { get; set; }
        public int QuestionCategoryId { get; set; }
        public double NegativePoints { get; set; }
        public string QuestionText { get; set; }
        public double QuestionPoints { get; set; }
        public int? PackageId { get; set; }
        public ICollection<Answer> Answers { get; set; }
        public ICollection<QuestionsInPackage> QuestionsInPackages { get; set; }
        public byte[]? FileData { get; set; }
        public string PackageName { get; set; }
        public string FileType { get; set; }
    }
}
