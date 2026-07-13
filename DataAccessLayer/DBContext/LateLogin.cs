using System;
using System.Collections.Generic;

namespace DataAccessLayer.DBContext;

public partial class LateLogin
{
    public int LateLoginId { get; set; }

    public int CompanyId { get; set; }

    public int RegionId { get; set; }

    public string LateLogin1 { get; set; } = null!;

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public int EmployeeId { get; set; }

    public int UserId { get; set; }

    public int ManagerId { get; set; }

    public DateOnly RequestDate { get; set; }

    public TimeOnly RequestedLateLoginTime { get; set; }

    public string Reason { get; set; } = null!;

    public string? ManagerRemarks { get; set; }

    public string Status { get; set; } = null!;

    public string? HrEmail { get; set; }

    public virtual Company Company { get; set; } = null!;

    public virtual User Employee { get; set; } = null!;

    public virtual User Manager { get; set; } = null!;

    public virtual Region Region { get; set; } = null!;
}
