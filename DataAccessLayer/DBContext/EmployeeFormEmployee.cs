using System;
using System.Collections.Generic;

namespace DataAccessLayer.DBContext;

public partial class EmployeeFormEmployee
{
    public int Id { get; set; }

    public int FormId { get; set; }

    public string? EmployeeCode { get; set; }

    public string? EmployeeName { get; set; }

    public virtual EmployeeForm Form { get; set; } = null!;
}
