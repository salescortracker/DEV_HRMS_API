using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.DTOs
{
    public class UserCreateDto
    {
        public int userId { get; set; }
        public int CompanyID { get; set; }
        public string? CompanyName { get; set; }

        public int RegionID { get; set; }
        public string? RegionName { get; set; }

        public string EmployeeCode { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty; // plain text for creation (hashed before save)
        public int RoleId { get; set; }
        public string? RoleName { get; set; }

        public int reportingTo { get; set; }
        public string? ReportingToName { get; set; }

        public int departmentId { get; set; }
        public string? DepartmentName { get; set; }

        public int? UserCompanyId { get; set; }
        public string loginType { get; set; }
        public string Status { get; set; } = "Active";
        public string? reportingmanagername { get; set; }
        public int? DesignationId { get; set; }
        public string? DesignationName { get; set; }

        public int? ReportingHR { get; set; }
        public string? ReportingHRName { get; set; }

        public DateOnly? JoiningDate { get; set; }
    }

    public class UserUpdateDto
    {
        public int UserID { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public int reportingTo { get; set; }
        public int departmentId { get; set; }
        public string Status { get; set; } = "Active"; // or "Inactive"
        public string loginType { get; set; }
        public int? DesignationId { get; set; }
        public int? ReportingHR { get; set; }
        public DateOnly? JoiningDate { get; set; }
    }

    public class UserReadDto
    {
        public int UserID { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public int? CompanyID { get; set; }
        public int? RegionID { get; set; }
        public int? ReportingHR { get; set; }
        public DateOnly? JoiningDate { get; set; }
    }
}
