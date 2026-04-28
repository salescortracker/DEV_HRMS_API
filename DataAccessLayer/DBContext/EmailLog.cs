using System;
using System.Collections.Generic;

namespace DataAccessLayer.DBContext;

public partial class EmailLog
{
    public int Id { get; set; }

    public int? EmployeeId { get; set; }

    public int? TemplateId { get; set; }

    public DateOnly? SentDate { get; set; }

    public string? Status { get; set; }

    public string? ErrorMessage { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }
}
