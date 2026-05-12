using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.DTOs
{
    public class HrApproveRejectDto
    {
        public List<int> PayrollIds { get; set; }
        public string Action { get; set; }
    }
}
