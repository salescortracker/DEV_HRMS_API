using System;
using System.Collections.Generic;

namespace DataAccessLayer.DBContext;

public partial class EmployeeFormEmployeeFile
{
    public int Id { get; set; }

    public int? FormId { get; set; }

    public string? EmployeeCode { get; set; }

    public string? FileName { get; set; }

    public string? FilePath { get; set; }

    public string? EmployeeName { get; set; }

    public string? Status { get; set; }
}
