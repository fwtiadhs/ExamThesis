﻿using System;
using System.Collections.Generic;

namespace ExamThesis.Models;

public partial class QuestionPackage
{
    public int PackageId { get; set; }

    public string? PackageName { get; set; }

    public int? QuestionCategoryId { get; set; }

    public virtual QuestionCategory? QuestionCategory { get; set; }
}
