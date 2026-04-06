using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.DTOs
{
    public class UserLeaveAllocationDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string DesignationName { get; set; }
        public string GradeName { get; set; }
        public string LeaveTypeName { get; set; }
        public int LeaveDays { get; set; }

        public int AllocatedLeaves { get; set; }   // from Grade
        public int ApprovedLeaves { get; set; }    // from LeaveRequests
        public int PendingLeaves { get; set; }     // from LeaveRequests
        public int RemainingLeaves { get; set; }
    }
}
