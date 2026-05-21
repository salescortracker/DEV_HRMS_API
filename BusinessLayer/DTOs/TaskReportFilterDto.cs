using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.DTOs
{
    public class TaskReportFilterDto
    {
        public string? EmployeeName { get; set; }
        public int? StatusId { get; set; }
        public int? PriorityId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
