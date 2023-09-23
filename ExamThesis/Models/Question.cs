namespace ExamThesis.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class Question
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Το πεδίο 'Κατηγορία Ερώτησης' είναι υποχρεωτικό.")]
    public string QuestionId { get; set; }

    public string ImageUrl { get; set; } // Προαιρετική εικόνα

    [Required(ErrorMessage = "Το πεδίο 'Τύπος Ερώτησης' είναι υποχρεωτικό.")]
    public QuestionType QuestionType { get; set; }

    public string FilePath { get; set; } // Προαιρετικό αρχείο

    [Required(ErrorMessage = "Το πεδίο 'Πόντοι Ερώτησης' είναι υποχρεωτικό.")]
    [Range(0, int.MaxValue, ErrorMessage = "Οι πόντοι πρέπει να είναι μεταξύ 0 και 2,147,483,647.")]
    public int QuestionPoints { get; set; }

    [Required(ErrorMessage = "Το πεδίο 'Πόντοι Αρνητικής Βαθμολογίας' είναι υποχρεωτικό.")]
    [Range(0, int.MaxValue, ErrorMessage = "Οι πόντοι πρέπει να είναι μεταξύ 0 και 2,147,483,647.")]
    public int NegativePoints { get; set; }

    public List<Answer> Answers { get; set; } // Λίστα με τις απαντήσεις
}



public enum QuestionType
{
    ΠολλαπλήςΕπιλογής,
    ΣωστόΛάθος,
    // Άλλοι τύποι ερωτήσεων που μπορεί να χρειάζεστε
}
