using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.DTOs
{
    public class MissedPunchApprovalListDto
    {
        public int MissedPunchRequestId { get; set; }
        public int UserId { get; set; }
        public string EmployeeName { get; set; }
        public DateOnly MissedDate { get; set; }
        public string MissedType { get; set; }
        public TimeOnly? CorrectClockIn { get; set; }
        public TimeOnly? CorrectClockOut { get; set; }
        public string Reason { get; set; }
        public string? HrEmail { get; set; }
    }
}
