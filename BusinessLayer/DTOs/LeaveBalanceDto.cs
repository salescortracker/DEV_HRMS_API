using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.DTOs
{
    public class LeaveBalanceDto
    {
        public int LeaveTypeId { get; set; }
        public string LeaveTypeName { get; set; }

        public int AllocatedLeaves { get; set; }

        public decimal ApprovedLeaves { get; set; }

        public decimal PendingLeaves { get; set; }

        public decimal RejectedLeaves { get; set; }

        public decimal RemainingLeaves { get; set; }
    }
}
