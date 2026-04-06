using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.DTOs
{
    public class ClockInOutFixDto
    {
        public int RegionId { get; set; }
        public int CompanyId { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;

        // ✅ FIX: Use DateOnly (same as DB)
        public DateOnly AttendanceDate { get; set; }

        public TimeOnly? ClockInTime { get; set; }
        public TimeOnly? ClockOutTime { get; set; }

        public string ActionType { get; set; } = string.Empty;
        public TimeOnly ActionTime { get; set; }

        public int CreatedBy { get; set; }
    }
}
