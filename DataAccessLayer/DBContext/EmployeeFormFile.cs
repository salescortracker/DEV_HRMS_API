using System;
using System.Collections.Generic;

namespace DataAccessLayer.DBContext;

public partial class EmployeeFormFile
{
    public int Id { get; set; }

    public int FormId { get; set; }

    public string? FileName { get; set; }

    public string? FilePath { get; set; }

    public virtual EmployeeForm Form { get; set; } = null!;
}
