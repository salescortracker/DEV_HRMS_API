using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.DTOs
{
    public class SendPayslipRangeDto
    {
        public List<int> PayrollIds { get; set; }
        public string Email { get; set; }

        public int FromMonth { get; set; }
        public int ToMonth { get; set; }
        public int Year { get; set; }
    }
}
