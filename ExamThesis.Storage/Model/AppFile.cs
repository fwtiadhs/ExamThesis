using System;
using System.Collections.Generic;

namespace ExamThesis.Storage.Model;

public partial class AppFile
{
    public int Id { get; set; }

    public byte[] Content { get; set; } = null!;
}
