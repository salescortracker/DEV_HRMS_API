using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.DTOs
{
    public class UserDesignationDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string EmployeeCode { get; set; } = string.Empty;

        public int CompanyId { get; set; }
        public int RegionId { get; set; }

        public int DesignationId { get; set; }
        public string DesignationName { get; set; } = string.Empty;
    }
}
