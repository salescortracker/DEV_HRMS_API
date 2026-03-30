using System;
using System.Collections.Generic;

namespace DataAccessLayer.DBContext;

public partial class AssetAssignment
{
    public int AssignmentId { get; set; }

    public int CompanyId { get; set; }

    public int RegionId { get; set; }

    public int UserId { get; set; }

    public int RequestId { get; set; }

    public int AssetId { get; set; }

    public string? EmployeeName { get; set; }

    public string? AssetType { get; set; }

    public string? AssetName { get; set; }

    public string? AssetCode { get; set; }

    public DateOnly AssignDate { get; set; }

    public DateOnly? ReturnDate { get; set; }

    public string? Remarks { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? Status { get; set; }
}
