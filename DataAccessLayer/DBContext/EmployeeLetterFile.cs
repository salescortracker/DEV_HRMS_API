using System;
using System.Collections.Generic;

namespace DataAccessLayer.DBContext;

public partial class EmployeeLetterFile
{
    public int Id { get; set; }

    public int LetterId { get; set; }

    public string? FileName { get; set; }

    public string? FilePath { get; set; }

    public virtual EmployeeLetter Letter { get; set; } = null!;
}
