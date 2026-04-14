using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.DTOs
{
    public class PayslipFilterDto
    {
        public int UserId { get; set; }
        public int EmployeeId { get; set; }
        public int CompanyId { get; set; }
        public int RegionId { get; set; }
        public int FromMonth { get; set; }
        public int ToMonth { get; set; }
        public int Year { get; set; }

    }

    public class SendPayslipDto
    {
        public List<int> PayrollIds { get; set; }   // 🔥 NEW
        public string Email { get; set; }

        public int FromMonth { get; set; }          // 🔥 NEW
        public int ToMonth { get; set; }            // 🔥 NEW
        public int Year { get; set; }               // 🔥 NEW
    }
}
