namespace ExamThesis.Models;

using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
/*
public class QuestionModel : Question 
{ 

    [BindProperty]
    public Question FileUpload { get; set; }

  
}
*/
public class Question
{
    public int QuestionCategoryId { get; set; }

    public int QuestionTypeId { get; set; }

    public int QuestionPoints { get; set; }

    public double NegativePoints { get; set; }

    public string QuestionText { get; set; } = null!;



    /*public string QuestionText { get; set; }

    
    [Required(ErrorMessage = "Το πεδίο 'Κατηγορία Ερώτησης' είναι υποχρεωτικό.")]
    public int QuestionCategoryId { get; set; }
    [NotMapped]
    [Display(Name = "Image")]
    public IFormFile ImageUrl { get; set; } // Προαιρετική εικόνα

    [Required(ErrorMessage = "Το πεδίο 'Τύπος Ερώτησης' είναι υποχρεωτικό.")]
    public QuestionType QuestionType { get; set; }

    /*[NotMapped]
    [Display(Name = "File")]
    public IFormFile FilePath { get; set; } // Προαιρετικό αρχείο

    [Required(ErrorMessage = "Το πεδίο 'Πόντοι Ερώτησης' είναι υποχρεωτικό.")]
    public int QuestionPoints { get; set; }

    [Required(ErrorMessage = "Το πεδίο 'Πόντοι Αρνητικής Βαθμολογίας' είναι υποχρεωτικό.")]
    public double NegativePoints { get; set; }

    public List<Answer> Answers { get; set; } // Λίστα με τις απαντήσεις */
}
    [Keyless]
public class QuestionAnswer {
    public Question Question { get; set; }
    public Answer Answer { get; set; }
    public bool IsCorrect { get; set; }
}


public class QuestionType
{
    [Key]
    public int QuestionTypeId { get; set; }
    public string QuestionTypeName { get; set; }
    // Άλλοι τύποι ερωτήσεων που μπορεί να χρειάζεστε
}
