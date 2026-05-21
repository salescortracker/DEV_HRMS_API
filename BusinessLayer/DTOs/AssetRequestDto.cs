namespace BusinessLayer.DTOs
{
   public class AssetRequestDto
    {
        public int? RequestID { get; set; }

        public int CompanyID { get; set; }
        public int RegionID { get; set; }
        public int UserID { get; set; }

        public string? EmployeeName { get; set; }
        public string? EmployeeCode { get; set; }
        public string? Department { get; set; }
        public string? DepartmentName { get; set; }

        public int AssetType { get; set; }
        public int? AssetCategory { get; set; }

        public DateTime RequiredDate { get; set; }
        public int? Priority { get; set; }

        public string Reason { get; set; } = string.Empty;

        public string? FileName { get; set; }
        public string? FilePath { get; set; }

        public int? ReportingTo { get; set; }

        public string? Status { get; set; }
        public string? HrEmail { get; set; }
    }
}
