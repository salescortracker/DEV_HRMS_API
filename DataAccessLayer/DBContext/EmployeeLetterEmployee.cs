using System;
using System.Collections.Generic;

namespace DataAccessLayer.DBContext;

public partial class EmployeeLetterEmployee
{
    public int Id { get; set; }

    public int LetterId { get; set; }

    public string? EmployeeCode { get; set; }

    public string? EmployeeName { get; set; }

    public virtual EmployeeLetter Letter { get; set; } = null!;
}
