using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.DTOs
{
    public class PayrollTransactionDto
    {
        public int PayrollId { get; set; }
        public int EmployeeId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal GrossSalary { get; set; }
        public decimal TotalDeductions { get; set; }
        public decimal NetSalary { get; set; }
        public string Status { get; set; }

        public int UserId { get; set; }
        public string? CompanyId { get; set; }
        public string? RegionId { get; set; }
        public List<PayrollDetailDto> Details { get; set; }
        // Attendance fields
        public int WorkingDays { get; set; }

        public int PresentDays { get; set; }

        public int LeaveDays { get; set; }

        public int HalfDays { get; set; }

        // Expenses
        public decimal AttendanceDeduction { get; set; }

        public decimal Expenses { get; set; }

        public bool? IsDownloadApproved { get; set; }
        public string? RequestStatus { get; set; }
        public string? HrEmail { get; set; }
        public string? EmployeeName { get; set; }

        public string Designation { get; set; }
        public string Department { get; set; }
        public string Location { get; set; }
        public DateTime? JoiningDate { get; set; }

        public string EmployeeCode { get; set; }
        public string Bank { get; set; }
        public string AccountNo { get; set; }
        public string Pan { get; set; }
        public int LateCount { get; set; } // ✅ ADD THIS
        public decimal LateDeduction { get; set; }

    }
}


