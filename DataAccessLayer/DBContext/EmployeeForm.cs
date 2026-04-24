using System;
using System.Collections.Generic;

namespace DataAccessLayer.DBContext;

public partial class EmployeeForm
{
    public int Id { get; set; }

    public int RegionId { get; set; }

    public int CompanyId { get; set; }

    public int UserId { get; set; }

    public int DocumentTypeId { get; set; }

    public string DocumentName { get; set; } = null!;

    public DateOnly IssueDate { get; set; }

    public string? Remarks { get; set; }

    public bool IsConfidential { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public string? Status { get; set; }

    public virtual AttachmentType DocumentType { get; set; } = null!;

    public virtual ICollection<EmployeeFormEmployee> EmployeeFormEmployees { get; set; } = new List<EmployeeFormEmployee>();

    public virtual ICollection<EmployeeFormFile> EmployeeFormFiles { get; set; } = new List<EmployeeFormFile>();
}
