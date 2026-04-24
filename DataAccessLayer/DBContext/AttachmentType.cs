using System;
using System.Collections.Generic;

namespace DataAccessLayer.DBContext;

public partial class AttachmentType
{
    public int AttachmentTypeId { get; set; }

    public int CompanyId { get; set; }

    public int RegionId { get; set; }

    public string AttachmentTypeName { get; set; } = null!;

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public string AttachmentCategory { get; set; } = null!;

    public virtual Company Company { get; set; } = null!;

    public virtual ICollection<EmployeeForm> EmployeeForms { get; set; } = new List<EmployeeForm>();

    public virtual ICollection<EmployeeLetter> EmployeeLetters { get; set; } = new List<EmployeeLetter>();

    public virtual Region Region { get; set; } = null!;
}
