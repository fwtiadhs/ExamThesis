using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamThesis.Common
{
    public class CreateExam : IValidatableObject
    {
        [Required(ErrorMessage = "Η ώρα έναρξης είναι υποχρεωτική.")]
        public DateTime StartTime { get; set; }
        [Required(ErrorMessage = "Η ώρα λήξης είναι υποχρεωτική.")]
        public DateTime EndTime { get; set; }
        [Required(ErrorMessage = "Οι συνολικοί πόντοι είναι υποχρεωτικοί.")]
        [Range(1, double.MaxValue, ErrorMessage = "Οι συνολικοί πόντοι πρέπει να είναι μεγαλύτεροι από το μηδέν.")]
        public double TotalPoints { get; set; }
        public int ExamId { get; set; }
        [Required(ErrorMessage = "Η ελάχιστη βάση εξέτασης είναι υποχρεωτική.")]
        public double? PassGrade { get; set; }
        [Required(ErrorMessage = "Το όνομα της εξέτασης είναι υποχρεωτικό.")]
        public string? ExamName { get; set; }
        [Required(ErrorMessage = "Πρέπει να επιλέξετε τουλάχιστον μία κατηγορία.")]
        public List<QuestionCategory> SelectedCategories { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (EndTime <= StartTime)
            {
                yield return new ValidationResult(
                    "Η ώρα λήξης πρέπει να είναι μεταγενέστερη της ώρας έναρξης.",
                    new[] { nameof(EndTime) }
                );
            }
            if (SelectedCategories != null && !SelectedCategories.Any(c => c.IsChecked))
            {
                yield return new ValidationResult(
                    "Πρέπει να επιλέξετε τουλάχιστον μία κατηγορία.",
                    new[] { nameof(SelectedCategories) }
                );

            }
        }
    }
}
