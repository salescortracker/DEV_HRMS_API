using System;
using System.Collections.Generic;

namespace DataAccessLayer.DBContext;

public partial class EmployeeNotification
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int FormId { get; set; }

    public string? Message { get; set; }

    public bool? IsRead { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string EmployeeCode { get; set; } = null!;
}
