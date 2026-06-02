using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.DTOs
{
    public class UserProfileDto
    {
        public int UserId { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public string FullName { get; set; } = null!;
        public int reportingTo { get; set; }
        public int? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public int CompanyID { get; set; }
        public int RegionID { get; set; }
    }
}
