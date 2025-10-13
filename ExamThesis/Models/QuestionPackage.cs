using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExamThesis.Models;

public partial class QuestionPackage
{
    public int PackageId { get; set; }

    [Required(ErrorMessage = "Το όνομα πακέτου είναι υποχρεωτικό.")]
    public string? PackageName { get; set; }
    public IFormFile? FileData { get; set; }
    public int? QuestionCategoryId { get; set; }
    public string? FileType { get; set; }
    public virtual QuestionCategory? QuestionCategory { get; set; }
}
