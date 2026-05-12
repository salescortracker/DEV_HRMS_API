using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.DTOs
{
    public class HrRequestDto
    {
        public List<int> PayrollIds { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public int FromMonth { get; set; }
        public int ToMonth { get; set; }
        public string FromMonthName { get; set; }
        public string ToMonthName { get; set; }
        public int Year { get; set; }

    }
}
