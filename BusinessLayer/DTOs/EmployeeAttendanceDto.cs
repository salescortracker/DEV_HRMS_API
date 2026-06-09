using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.DTOs
{
    public class EmployeeAttendanceDto
    {
        public int AttendanceId { get; set; }

        public int RegionId { get; set; }
        public int CompanyId { get; set; }

        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }

        public DateTime AttendanceDate { get; set; }

        public string Status { get; set; }
        public string ClockIn { get; set; }   // ADD
        public string ClockOut { get; set; }  // ADD
        public string GrossTime { get; set; }

        public string? ShiftName { get; set; }

        public string? ShiftStartTime { get; set; }

        public string? ShiftEndTime { get; set; }

        public int? LateMinutes { get; set; }

        public string? ArrivalStatus { get; set; }

    }
}
