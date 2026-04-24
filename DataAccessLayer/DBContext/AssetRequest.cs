using System;
using System.Collections.Generic;

namespace DataAccessLayer.DBContext;

public partial class AssetRequest
{
    public int RequestId { get; set; }

    public int CompanyId { get; set; }

    public int RegionId { get; set; }

    public int UserId { get; set; }

    public string? EmployeeName { get; set; }

    public string? EmployeeCode { get; set; }

    public string? Department { get; set; }

    public int AssetTypeId { get; set; }

    public int? AssetCategoryId { get; set; }

    public DateOnly RequiredDate { get; set; }

    public int? PriorityId { get; set; }

    public string Reason { get; set; } = null!;

    public string FileName { get; set; } = null!;

    public string? FilePath { get; set; }

    public int? ReportingTo { get; set; }

    public string Status { get; set; } = null!;

    public int? CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public string? HrEmail { get; set; }
}
