using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.DTOs
{
    public class HrPayrollFilterDto
    {
        public int CompanyId { get; set; }
        public int RegionId { get; set; }
        public int? EmployeeId { get; set; }
        public int FromMonth { get; set; }
        public int ToMonth { get; set; }
        public int Year { get; set; }
    }
}
