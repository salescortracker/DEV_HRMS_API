using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.DTOs
{
    public class CreateEarlyLogoutRequestDto
    {
        public int EmployeeID { get; set; }
        public int UserId { get; set; }
        public DateOnly RequestDate { get; set; }
        public TimeOnly RequestedLogoutTime { get; set; }
        public string Reason { get; set; } = null!;
        public string? HrEmail { get; set; }
        public int CompanyID { get; set; }
        public int RegionID { get; set; }
        public int reportingTo { get; set; }
    }

    public class UpdateEarlyLogoutDto
    {
        public int EarlyLogoutRequestID { get; set; }
        public int CompanyID { get; set; }
        public int? RegionID { get; set; }
        public DateOnly RequestDate { get; set; }
        public TimeOnly RequestedLogoutTime { get; set; }
        public string Reason { get; set; } = null!;
        public string? HrEmail { get; set; }
    }

    public class BulkApproveRejectEarlyLogoutDto
    {
        public List<int> EarlyLogoutRequestIds { get; set; } = new();
        public string Status { get; set; } = null!;
        public string? ManagerRemarks { get; set; }
        public int ManagerID { get; set; }
        public int CompanyID { get; set; }
        public int? RegionID { get; set; }
        public string? HrEmail { get; set; }
    }

    public class EarlyLogoutApprovalListDto
    {
        public int EarlyLogoutRequestId { get; set; }
        public int UserId { get; set; }
        public string EmployeeName { get; set; } = null!;
        public DateOnly RequestDate { get; set; }
        public TimeOnly RequestedLogoutTime { get; set; }
        public string Reason { get; set; } = null!;
        public string? HrEmail { get; set; }
        public string Status { get; set; } = null!;
        public string? ManagerRemarks { get; set; }
    }
}
