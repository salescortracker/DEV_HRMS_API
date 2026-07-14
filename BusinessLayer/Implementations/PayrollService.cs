using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Implementations
{
    public class PayrollService: IPayrollService
    {
        private readonly HRMSContext _context;
        private readonly IEmailService _emailService;
        private readonly INotificationService _notificationService;

        public PayrollService(HRMSContext context, IEmailService emailService, INotificationService notificationService)
        {
            _context = context;
            _emailService = emailService;
            _notificationService = notificationService;
        }

        /* ============================================================
             ATTENDANCE SUMMARY (UPDATED WITH LATE LOGIC)
        ============================================================ */

        private async Task<(int workingDays,
                            int presentDays,
                            int leaveDays,
                            int lopDays,
                            int halfDays,
                            int lateCount,
                            decimal lateDeductionDays)>
        GetEmployeeAttendanceSummary(int employeeId, int userId, int month, int year)
        {
            var employee = await _context.Users
                .Where(x => x.UserId == employeeId)
                .Select(x => new
                {
                    x.EmployeeCode,
                    x.CompanyId,
                    x.RegionId,
                    x.UserId // ✅ FIX
                })
                .FirstOrDefaultAsync();

            if (employee == null)
                return (0, 0, 0, 0, 0, 0, 0m); // ✅ FIX

            DateOnly startDate = new DateOnly(year, month, 1);
            DateOnly endDate = new DateOnly(year, month, DateTime.DaysInMonth(year, month));

            var attendance = await _context.EmployeeAttendances
                .Where(a =>
                    a.EmployeeCode == employee.EmployeeCode &&
                    a.CompanyId == employee.CompanyId &&
                    a.RegionId == employee.RegionId &&
                    a.AttendanceDate >= startDate &&
                    a.AttendanceDate <= endDate)
                .ToListAsync();

            int present = attendance.Count(a => a.Status == "Present");

            int leave = attendance.Count(a =>
                a.Status == "SickLeave" ||
                a.Status == "CasualLeave" ||
                a.Status == "PaidLeave");
            int lopDays = attendance.Count(a =>
    a.Status == "LOP");

            int manualHalfDays = attendance.Count(a => a.Status == "HalfDay");

            int lateArrivals = attendance
                .Where(a => a.LateMinutes.HasValue && a.LateMinutes.Value > 0)
                .GroupBy(a => a.AttendanceDate)
                .Count();

            // ✅ POLICY CALL FIX
            var policy = await GetLateLoginPolicy(
                userId,
                employee.CompanyId,
                employee.RegionId
            );

            decimal lateDeductionDays = 0;

            if (policy != null && policy.LateLoginCount > 0)
            {
                int blocks = lateArrivals / policy.LateLoginCount;

                if (policy.Loptype?.ToLower() == "half day")
                    lateDeductionDays = blocks * 0.5m;
                else if (policy.Loptype?.ToLower() == "full day")
                    lateDeductionDays = blocks * 1m;
            }

            // ✅ FIX half calculation
            int lateHalfDays = (int)Math.Round(lateDeductionDays * 2);
            //int half = manualHalfDays + lateHalfDays;
            int half = manualHalfDays;

            int totalDays = DateTime.DaysInMonth(year, month);

            int weekendDays = Enumerable.Range(1, totalDays)
                .Select(d => new DateTime(year, month, d))
                .Count(d => d.DayOfWeek == DayOfWeek.Saturday ||
                            d.DayOfWeek == DayOfWeek.Sunday);

            int workingDays = totalDays - weekendDays;

            return (
    workingDays,
    present,
    leave,
    lopDays,
    half,
    lateArrivals,
    lateDeductionDays
);
        }

        /* ============================================================
           EXPENSE CALCULATION
        ============================================================ */

        private async Task<decimal> GetApprovedExpenses(int employeeId, int month, int year)
        {
            var expenses = await _context.Expenses
                .Where(e =>
                    e.UserId == employeeId &&
                    e.Status == "Approved" &&
                    e.ExpenseDate.HasValue &&
                    e.ExpenseDate.Value.Month == month &&
                    e.ExpenseDate.Value.Year == year)
                .SumAsync(e => (decimal?)e.Amount);

            return expenses ?? 0;
        }

        /* ============================================================
           COMMON PAYROLL CALCULATION
        ============================================================ */

        private async Task<(decimal gross, decimal totalDeduction, decimal attendanceDeduction, decimal expenses, List<PayrollDetail> details)>
        CalculatePayroll(
            EmployeeSalary empSalary,
            List<SalaryStructureComponent> structureComponents,
            int userId,
            int month,
            int year)
        {
            decimal basic = 0;
            decimal gross = 0;
            decimal totalDeduction = 0;

            var payrollDetails = new List<PayrollDetail>();

            /* ================= BASIC ================= */

            var basicComponent = structureComponents
                .FirstOrDefault(x => x.Component.ComponentName.ToLower() == "basic");

            if (basicComponent != null)
            {
                if (basicComponent.CalculationType?.ToLower() == "fixed")
                    basic = basicComponent.Value;
                else if (basicComponent.CalculationType?.ToLower() == "percentage")
                    basic = empSalary.Ctc * basicComponent.Value / 100;

                basic = Math.Round(basic, 2);
                gross += basic;

                payrollDetails.Add(CreatePayrollDetail(basicComponent.ComponentId, basic, userId));
            }

            /* ================= EARNINGS ================= */

            var earnings = structureComponents
                .Where(x => x.Component.Type == "Earning" &&
                            x.Component.ComponentName.ToLower() != "basic");

            foreach (var item in earnings)
            {
                decimal amount = 0;

                if (item.CalculationType?.ToLower() == "fixed")
                    amount = item.Value;

                else if (item.CalculationType?.ToLower() == "percentage")
                {
                    if (item.Component.PercentageOf?.ToLower() == "basic")
                        amount = basic * item.Value / 100;
                    else
                        amount = empSalary.Ctc * item.Value / 100;
                }

                amount = Math.Round(amount, 2);
                gross += amount;

                payrollDetails.Add(CreatePayrollDetail(item.ComponentId, amount, userId));
            }

            /* ================= DEDUCTIONS ================= */

            var deductions = structureComponents
                .Where(x => x.Component.Type == "Deduction");

            foreach (var item in deductions)
            {
                decimal amount = 0;

                if (item.CalculationType?.ToLower() == "fixed")
                    amount = item.Value;

                else if (item.CalculationType?.ToLower() == "percentage")
                {
                    if (item.Component.PercentageOf?.ToLower() == "basic")
                        amount = basic * item.Value / 100;
                    else
                        amount = empSalary.Ctc * item.Value / 100;
                }

                amount = Math.Round(amount, 2);
                totalDeduction += amount;

                payrollDetails.Add(CreatePayrollDetail(item.ComponentId, amount, userId));
            }

            /* ================= ATTENDANCE ================= */

            //        // UPDATED: now includes lateCount
            //        var attendance = await GetEmployeeAttendanceSummary(
            //            empSalary.EmployeeId, userId, month, year);
            //        decimal lateDeductionAmount =
            //attendance.lateDeductionDays * (attendance.workingDays == 0 ? 0 : gross / attendance.workingDays);

            //        int allowedLeaves = 1;
            //        int allowedHalfDays = 2;

            //        // Existing logic (no change needed)
            //        int extraLeaves = Math.Max(0, attendance.leaveDays - allowedLeaves);
            //        int extraHalfDays = Math.Max(0, attendance.halfDays - allowedHalfDays);

            //        decimal perDaySalary = attendance.workingDays == 0
            //            ? 0
            //            : gross / attendance.workingDays;

            //        decimal attendanceDeduction =
            //            (extraLeaves * perDaySalary) +
            //            (extraHalfDays * (perDaySalary / 2)) +
            //            lateDeductionAmount;

            //        attendanceDeduction = Math.Round(attendanceDeduction, 2);


            /* ================= ATTENDANCE ================= */

            var attendance = await GetEmployeeAttendanceSummary(
                empSalary.EmployeeId,
                userId,
                month,
                year
            );

            // =========================================
            // PER DAY SALARY
            // =========================================

            decimal perDaySalary =
                attendance.workingDays == 0
                ? 0
                : gross / attendance.workingDays;

            // =========================================
            // HALF DAY CALCULATION
            // 2 HALF DAYS = 1 DAY DEDUCTION
            // =========================================

            decimal halfDayDeduction =
                attendance.halfDays * 0.5m;

            // =========================================
            // LATE DEDUCTION DAYS
            // Example:
            // 5 Late = 0.5 Day
            // =========================================

            decimal lateDeductionDays =
                attendance.lateDeductionDays;

            // =========================================
            // FINAL PAYABLE DAYS
            // =========================================

            decimal payableDays =
                attendance.presentDays
                - attendance.leaveDays
                - attendance.lopDays
                - halfDayDeduction
                - lateDeductionDays;

            // Prevent negative salary
            if (payableDays < 0)
            {
                payableDays = 0;
            }

            // =========================================
            // ATTENDANCE DEDUCTION AMOUNT
            // =========================================

            decimal attendanceDeductionAmount =
                (attendance.leaveDays * perDaySalary)
                + (attendance.lopDays * perDaySalary)
                + (halfDayDeduction * perDaySalary)
                + (lateDeductionDays * perDaySalary);

            attendanceDeductionAmount =
                Math.Round(attendanceDeductionAmount, 2);

            // =========================================
            // ACTUAL EARNED SALARY
            // =========================================

            decimal earnedSalary =
                payableDays * perDaySalary;

            earnedSalary = Math.Round(earnedSalary, 2);

            // =========================================
            // EXPENSES
            // =========================================

            decimal expenses = await GetApprovedExpenses(
                empSalary.EmployeeId,
                month,
                year
            );

            if (expenses > 0)
            {
                earnedSalary += expenses;

                payrollDetails.Add(new PayrollDetail
                {
                    ComponentId = 0,
                    Amount = expenses,
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow
                });
            }

            // =========================================
            // DEDUCTIONS (PF/PT/etc)
            // =========================================

            decimal finalDeduction =
                totalDeduction;

            // =========================================
            // FINAL GROSS
            // =========================================

            gross = earnedSalary;





            /* ================= EXPENSES ================= */

            //        var expenses = await GetApprovedExpenses(
            //empSalary.EmployeeId, month, year);

            //        if (expenses > 0)
            //        {
            //            gross += expenses;

            //            payrollDetails.Add(new PayrollDetail
            //            {
            //                Amount = expenses,
            //                UserId = userId,
            //                CreatedAt = DateTime.UtcNow
            //            });
            //}

            //return (
            //    Math.Round(gross, 2),
            //    Math.Round(totalDeduction, 2),
            //    attendanceDeduction,
            //    expenses,
            //    payrollDetails
            //);







            return (
                Math.Round(gross, 2),
                Math.Round(finalDeduction, 2),
                Math.Round(attendanceDeductionAmount, 2),
                expenses,
                payrollDetails
            );
        }

        /* ============================================================
           PREVIEW PAYROLL
        ============================================================ */

        public async Task<List<PayrollTransactionDto>> PreviewPayrollAsync(ProcessPayrollRequestDto dto, int userId)
        {
            var resultList = new List<PayrollTransactionDto>();

            var activeSalaries = await _context.EmployeeSalaries
                .Where(x => x.IsActive && x.UserId == userId)
                .ToListAsync();

            foreach (var empSalary in activeSalaries)
            {
                var structureComponents = await _context.SalaryStructureComponents
                    .Include(x => x.Component)
                    .Where(x => x.StructureId == empSalary.StructureId)
                    .ToListAsync();

                var (gross, totalDeduction, attendanceDeduction, expenses, details) =
                    await CalculatePayroll(empSalary, structureComponents, userId, dto.Month, dto.Year);

                var attendance = await GetEmployeeAttendanceSummary(
                    empSalary.EmployeeId, userId, dto.Month, dto.Year);

                var existingPayroll = await _context.PayrollTransactions
                    .FirstOrDefaultAsync(x =>
                    x.EmployeeId == empSalary.EmployeeId &&
                    x.Month == dto.Month &&
                    x.Year == dto.Year &&
                    x.UserId == userId
                    );

                var status = existingPayroll != null ? "Processed" : "Preview";

                var detailList = details.Select(d => new PayrollDetailDto
                {
                    ComponentId = d.ComponentId,
                    Amount = d.Amount,
                    Type = structureComponents
                        .FirstOrDefault(x => x.ComponentId == d.ComponentId)?.Component.Type ?? "",
                    ComponentName = structureComponents
                        .FirstOrDefault(x => x.ComponentId == d.ComponentId)?.Component.ComponentName ?? "Other"
                }).ToList();

                resultList.Add(new PayrollTransactionDto
                {
                    EmployeeId = empSalary.EmployeeId,
                    Month = dto.Month,
                    Year = dto.Year,
                    GrossSalary = gross,
                    TotalDeductions = totalDeduction + attendanceDeduction,
                    NetSalary = gross - (totalDeduction + attendanceDeduction),
                    AttendanceDeduction = attendanceDeduction,
                    Expenses = expenses,
                    Status = status,
                    WorkingDays = attendance.workingDays,
                    PresentDays = attendance.presentDays,
                    LeaveDays = attendance.leaveDays,
                    HalfDays = attendance.halfDays,
                    Details = detailList,
                    LateCount = attendance.lateCount, // ✅ NEW
                });
            }

            return resultList;
        }

        /* ============================================================
           PROCESS PAYROLL
        ============================================================ */

        public async Task<string> ProcessPayrollAsync(ProcessPayrollRequestDto dto, int userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var activeSalaries = await _context.EmployeeSalaries
                    .Where(x => x.IsActive && x.UserId == userId)
                    .ToListAsync();

                foreach (var empSalary in activeSalaries)
                {
                    var alreadyProcessed = await _context.PayrollTransactions
                        .AnyAsync(x =>
                            x.EmployeeId == empSalary.EmployeeId &&
                            x.Month == dto.Month &&
                            x.Year == dto.Year &&
                            x.UserId == userId);

                    if (alreadyProcessed)
                        continue;

                    var structureComponents = await _context.SalaryStructureComponents
                        .Include(x => x.Component)
                        .Where(x => x.StructureId == empSalary.StructureId)
                        .ToListAsync();

                    var (gross, totalDeduction, attendanceDeduction, expenses, payrollDetails) =
                        await CalculatePayroll(empSalary, structureComponents, userId, dto.Month, dto.Year);

                    var payrollTransaction = new PayrollTransaction
                    {
                        EmployeeId = empSalary.EmployeeId,
                        Month = dto.Month,
                        Year = dto.Year,
                        GrossSalary = gross,
                        TotalDeductions = totalDeduction + attendanceDeduction,
                    //    AttendanceDeduction = attendanceDeduction,              // ✅ save separately
                        NetSalary = gross - (totalDeduction + attendanceDeduction),
                        Status = "Processed",
                        UserId = userId,
                        CompanyId = empSalary.CompanyId,
                        RegionId = empSalary.RegionId,
                        CreatedAt = DateTime.UtcNow,
                        IsDownloadApproved = false,
                        RequestStatus = "Not Requested",
                    };

                    _context.PayrollTransactions.Add(payrollTransaction);
                    await _context.SaveChangesAsync();

                    foreach (var detail in payrollDetails)
                    {
                        detail.PayrollId = payrollTransaction.PayrollId;
                        _context.PayrollDetails.Add(detail);
                    }

                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();
                return "Payroll Processed Successfully";
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        /* ============================================================
           GET PAYROLL BY MONTH
        ============================================================ */

        public async Task<List<PayrollTransactionDto>> GetPayrollByMonthAsync(int month, int year, int userId)
        {
            return await _context.PayrollTransactions
                .Where(x => x.Month == month && x.Year == year && x.UserId == userId)
                .Select(x => new PayrollTransactionDto
                {
                    PayrollId = x.PayrollId,
                    EmployeeId = x.EmployeeId,
                    Month = x.Month,
                    Year = x.Year,
                    GrossSalary = x.GrossSalary,
                    TotalDeductions = x.TotalDeductions,
                    NetSalary = x.NetSalary,
                    Status = x.Status
                })
                .ToListAsync();
        }

        /* ============================================================
           HELPER
        ============================================================ */

        private PayrollDetail CreatePayrollDetail(int componentId, decimal amount, int userId)
        {
            return new PayrollDetail
            {
                ComponentId = componentId,
                Amount = Math.Round(amount, 2),
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };
        }


        /* ============================================================
          Get Payslips By Range for employee
       ============================================================ */

        public async Task<List<PayrollTransactionDto>> GetPayslipsByRange(PayslipFilterDto dto)
        {
            var transactions = await _context.PayrollTransactions
                .Where(x =>
                    x.EmployeeId == dto.EmployeeId &&
                    x.CompanyId == dto.CompanyId.ToString() &&
                    x.RegionId == dto.RegionId.ToString() &&
                    x.Year == dto.Year &&
                    x.Month >= dto.FromMonth &&
                    x.Month <= dto.ToMonth &&
                    x.Status == "Processed"
                )
                .ToListAsync();

            // 🔥 LOAD MASTER DATA ONCE
            var users = await _context.Users.ToListAsync();

            var departments = await _context.Departments
                .Where(d => d.CompanyId == dto.CompanyId && d.RegionId == dto.RegionId)
                .ToListAsync();

            var designations = await _context.Designations
    .Where(d => d.CompanyId == dto.CompanyId && d.RegionId == dto.RegionId)
    .ToListAsync();

            var personalDetails = await _context.EmployeePersonalDetails
                .Where(p => p.CompanyId == dto.CompanyId && p.RegionId == dto.RegionId)
                .ToListAsync();

            var result = new List<PayrollTransactionDto>();

            foreach (var trx in transactions)
            {
                // 🔥 USER
                var user = users.FirstOrDefault(u => u.UserId == trx.EmployeeId);

                // 🔥 BANK DETAILS (USING USERID)
                var bank = await _context.EmployeeBankDetails
                    .Where(b =>
                        b.UserId == trx.EmployeeId &&
                        b.CompanyId == dto.CompanyId &&
                        b.RegionId == dto.RegionId
                    )
                    .OrderByDescending(b => b.CreatedAt)
                    .FirstOrDefaultAsync();

                // 🔥 PERSONAL DETAILS
                var personal = personalDetails.FirstOrDefault(p =>
                    p.UserId == trx.EmployeeId &&
                    p.CompanyId == dto.CompanyId &&
                    p.RegionId == dto.RegionId
                );

                // 🔥 DEPARTMENT NAME
                var departmentName = departments
                    .FirstOrDefault(d => d.DepartmentId == user?.DepartmentId)
                    ?.DepartmentName;

                var designationName = designations
    .FirstOrDefault(d => d.DesignationId == user?.DesignationId)
    ?.DesignationName;

                // 🔥 DESIGNATION (DIRECT STRING)
                // var designationName = user?.Designation;

                // 🔥 PAYROLL DETAILS (EARNINGS + DEDUCTIONS)
                var details = await _context.PayrollDetails
                    .Where(d => d.PayrollId == trx.PayrollId)
                    .ToListAsync();

                var componentIds = details.Select(d => d.ComponentId).ToList();

                var components = await _context.SalaryComponents
                    .Where(c => componentIds.Contains(c.ComponentId))
                    .ToListAsync();

                var detailDtos = details.Select(d => new PayrollDetailDto
                {
                    ComponentId = d.ComponentId,
                    Amount = d.Amount,
                    ComponentName = components
                        .FirstOrDefault(c => c.ComponentId == d.ComponentId)?.ComponentName ?? "Other",
                    Type = components
                        .FirstOrDefault(c => c.ComponentId == d.ComponentId)?.Type ?? "Other"
                }).ToList();

                // 🔥 ATTENDANCE
                //            var attendance = await GetEmployeeAttendanceSummary(
                //                trx.EmployeeId, trx.UserId ?? 0, trx.Month, trx.Year);

                //            decimal attendanceDeduction = 0;

                //            int allowedLeaves = 1;
                //            int allowedHalfDays = 2;

                //            int extraLeaves = Math.Max(0, attendance.leaveDays - allowedLeaves);
                //            int extraHalfDays = Math.Max(0, attendance.halfDays - allowedHalfDays);

                //            decimal perDaySalary = attendance.workingDays == 0
                //                ? 0
                //                : trx.GrossSalary / attendance.workingDays;

                //            attendanceDeduction =
                //                (extraLeaves * perDaySalary) +
                //                (extraHalfDays * (perDaySalary / 2));
                //            decimal lateDeductionAmount =
                //attendance.lateDeductionDays * perDaySalary;

                //            attendanceDeduction += lateDeductionAmount;

                //            attendanceDeduction = Math.Round(attendanceDeduction, 2);
              //  decimal attendanceDeduction = trx.AttendanceDeduction;

                // 🔥 EXPENSES
                var expenses = await GetApprovedExpenses(
                    trx.EmployeeId, trx.Month, trx.Year);

                // 🔥 FINAL RESULT
                result.Add(new PayrollTransactionDto
                {
                    PayrollId = trx.PayrollId,
                    EmployeeId = trx.EmployeeId,
                    Month = trx.Month,
                    Year = trx.Year,
                    Status = trx.Status,

                    GrossSalary = trx.GrossSalary,
                    TotalDeductions = trx.TotalDeductions,
                    NetSalary = trx.NetSalary,

                //    AttendanceDeduction = attendanceDeduction,
                    Expenses = expenses,

                    // ✅ EMPLOYEE DETAILS
                    EmployeeName = user?.FullName,
                    Designation = designationName ?? "-",
                    Department = departmentName ?? "-",
                    Location = "Hyderabad",

                    // ✅ DATE OF JOINING (FIXED TYPE)
                    JoiningDate = personal?.DateOfJoining,

                    EmployeeCode = user?.EmployeeCode ?? "-",

                    // ✅ BANK DETAILS
                    Bank = bank?.BankName ?? "-",
                    AccountNo = bank?.AccountNumber ?? "-",

                    // ✅ PAN
                    Pan = personal?.Pannumber ?? "-",

                    // ✅ DETAILS
                    Details = detailDtos,

                    RequestStatus = trx.RequestStatus ?? "Not Requested",
                    IsDownloadApproved = trx.IsDownloadApproved ?? false,
                    HrEmail = trx.HrEmail
                });
            }

            return result;
        }

        /* ============================================================
         Send Payslip Email Request to HR
      ============================================================ */
        public async Task SendPayslipRequestEmail(SendPayslipDto dto)
        {
            if (dto.PayrollIds == null || !dto.PayrollIds.Any())
                throw new Exception("No payrolls selected");

            // 🔥 Get payrolls
            var payrolls = await _context.PayrollTransactions
                .Where(x => dto.PayrollIds.Contains(x.PayrollId))
                .ToListAsync();

            if (!payrolls.Any())
                throw new Exception("Payroll not found");

            // 🔥 Get employee
            var employee = await _context.Users
                .Where(x => x.UserId == payrolls.First().EmployeeId)
                .Select(x => new { x.FullName })
                .FirstOrDefaultAsync();

            string employeeName = employee?.FullName ?? "Employee";

            var culture = System.Globalization.CultureInfo.CurrentCulture;

            string fromMonthName = culture.DateTimeFormat.GetMonthName(dto.FromMonth);
            string toMonthName = culture.DateTimeFormat.GetMonthName(dto.ToMonth);

            string monthDisplay = dto.FromMonth == dto.ToMonth
                ? fromMonthName
                : $"{fromMonthName} to {toMonthName}";

            // 🔥 UPDATE STATUS IN DB
            foreach (var trx in payrolls)
            {
                trx.RequestStatus = "Pending";
                trx.IsDownloadApproved = false;
                trx.HrEmail = dto.Email; // 🔥 SAVE HR EMAIL HERE
            }

            await _context.SaveChangesAsync();
            

            if (!string.IsNullOrWhiteSpace(dto.Email))
            {
                var hrEmails = dto.Email
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim())
                    .Distinct()
                    .ToList();

                var employeeUser = await _context.Users
                    .FirstOrDefaultAsync(x => x.UserId == payrolls.First().EmployeeId);

                if (employeeUser != null)
                {
                    var hrUsers = await _context.Users
                        .Where(x =>
                            hrEmails.Contains(x.Email) &&
                            x.CompanyId == employeeUser.CompanyId &&
                            x.RegionId == employeeUser.RegionId)
                        .ToListAsync();

                    var notificationUsers = hrUsers
                        .Select(x => x.UserId)
                        .Distinct()
                        .ToList();

                    if (notificationUsers.Any())
                    {
                        await _notificationService.CreateNotificationAsync(
                            notificationUsers,
                            "Payslip Request",
                            $"{employeeUser.FullName} requested payslips for {monthDisplay} {dto.Year}.",
                            "Payroll",
                            payrolls.First().PayrollId
                        );
                    }
                }
            }

            // 🔥 SINGLE EMAIL FOR ALL MONTHS
            var body = $@"
<div style='font-family:Segoe UI, Arial, sans-serif; background-color:#f4f6f9; padding:20px;'>

  <div style='max-width:600px; margin:auto; background:#ffffff; border-radius:10px; overflow:hidden; box-shadow:0 4px 12px rgba(0,0,0,0.15);'>

    <!-- HEADER -->
    <div style='background:#dc3545; color:#ffffff; padding:18px; text-align:center; font-size:22px; font-weight:bold;'>
      Payslip Download Request
    </div>

    <!-- BODY -->
    <div style='padding:25px; color:#333; font-size:15px;'>

      <p>Dear HR,</p>

      <p>
        Employee <b>{employeeName}</b> has requested payslips.
      </p>

      <!-- TABLE -->
      <table style='width:100%; border-collapse:collapse; margin-top:15px; font-size:14px;'>

        <tr>
          <td style='padding:10px; border:1px solid #ddd; background:#f8f9fa; font-weight:bold;'>
            Employee Name
          </td>
          <td style='padding:10px; border:1px solid #ddd;'>
            {employeeName}
          </td>
        </tr>

        <tr>
          <td style='padding:10px; border:1px solid #ddd; background:#f8f9fa; font-weight:bold;'>
            Month
          </td>
          <td style='padding:10px; border:1px solid #ddd;'>
            {monthDisplay}
          </td>
        </tr>

        <tr>
          <td style='padding:10px; border:1px solid #ddd; background:#f8f9fa; font-weight:bold;'>
            Year
          </td>
          <td style='padding:10px; border:1px solid #ddd;'>
            {dto.Year}
          </td>
        </tr>

        <tr>
          <td style='padding:10px; border:1px solid #ddd; background:#f8f9fa; font-weight:bold;'>
            Total Payslips
          </td>
          <td style='padding:10px; border:1px solid #ddd;'>
            {dto.PayrollIds.Count}
          </td>
        </tr>

      </table>

      <p style='margin-top:20px;'>
        Kindly review and approve the request at your earliest convenience.
      </p>

    </div>

    <!-- FOOTER -->
    <div style='background:#f1f1f1; padding:12px; text-align:center; font-size:12px; color:#777;'>
      © {DateTime.Now.Year} Cortracker360 HRMS System
    </div>

  </div>

</div>";

            await _emailService.SendEmailAsync(
                dto.Email,
                "Payslip Range Request",
                body,
                null
            );
        }

        /* ============================================================
                   Get Pending Requests In Hr Screens
        ============================================================ */
        public async Task<List<HrRequestDto>> GetPendingRequests(int companyId, int regionId, string hrEmail)
        {
            var data = await _context.PayrollTransactions
                .Where(x =>
                    x.CompanyId.Trim() == companyId.ToString() &&
                    x.RegionId.Trim() == regionId.ToString() &&
                    x.HrEmail.ToLower() == hrEmail.ToLower() &&
                    x.RequestStatus.Trim().ToLower() == "pending"
                )
                .ToListAsync();

            var users = await _context.Users.ToListAsync();
            var culture = System.Globalization.CultureInfo.CurrentCulture;

            var result = data
                .GroupBy(x => new { x.EmployeeId, x.Year })
                .Select(g => new HrRequestDto
                {
                    PayrollIds = g.Select(x => x.PayrollId).ToList(),
                    EmployeeId = g.Key.EmployeeId,
                    EmployeeName = users.FirstOrDefault(u => u.UserId == g.Key.EmployeeId)?.FullName,
                    FromMonth = g.Min(x => x.Month),
                    ToMonth = g.Max(x => x.Month),
                    FromMonthName = culture.DateTimeFormat.GetMonthName(g.Min(x => x.Month)),
                    ToMonthName = culture.DateTimeFormat.GetMonthName(g.Max(x => x.Month)),
                    Year = g.Key.Year
                })
                .ToList();

            return result;
        }

        /* ============================================================
           Approve Reject to download the Payslips to Employee
        ============================================================ */
        public async Task ApproveRejectPayslips(HrApproveRejectDto dto)
        {
            var payrolls = await _context.PayrollTransactions
                .Where(x => dto.PayrollIds.Contains(x.PayrollId))
                .ToListAsync();

            if (!payrolls.Any())
                return;

            // 🔥 Get Employee Details
            var employee = await _context.Users
                .Where(x => x.UserId == payrolls.First().EmployeeId)
                .Select(x => new { x.FullName, x.Email })
                .FirstOrDefaultAsync();

            string employeeName = employee?.FullName ?? "Employee";
            string employeeEmail = employee?.Email;

            var culture = System.Globalization.CultureInfo.CurrentCulture;

            int fromMonth = payrolls.Min(x => x.Month);
            int toMonth = payrolls.Max(x => x.Month);

            string fromMonthName = culture.DateTimeFormat.GetMonthName(fromMonth);
            string toMonthName = culture.DateTimeFormat.GetMonthName(toMonth);

            string monthDisplay = fromMonth == toMonth
                ? fromMonthName
                : $"{fromMonthName} to {toMonthName}";

            int year = payrolls.First().Year;

            // 🔥 UPDATE DB
            foreach (var trx in payrolls)
            {
                if (dto.Action == "Approved")
                {
                    trx.RequestStatus = "Approved";
                    trx.IsDownloadApproved = true;
                }
                else
                {
                    trx.RequestStatus = "Rejected";
                    trx.IsDownloadApproved = false;
                }
            }

            await _context.SaveChangesAsync();
            
            var employeeUserId = payrolls
                .Select(x => x.EmployeeId)
                .FirstOrDefault();

            if (employeeUserId > 0)
            {
                await _notificationService.CreateNotificationAsync(
                    new List<int> { employeeUserId },
                    "Payslip Request",
                    $"Your payslip request has been {dto.Action}.",
                    "Payslip",
                    payrolls.First().PayrollId
                );
            }

            // 🔥 EMAIL TEMPLATE COLOR BASED ON ACTION
            string headerColor = dto.Action == "Approved" ? "#28a745" : "#dc3545";
            string statusText = dto.Action;

            // 🔥 EMAIL BODY (SAME STYLE AS YOUR CODE)
            var body = $@"
<div style='font-family:Segoe UI, Arial, sans-serif; background-color:#f4f6f9; padding:20px;'>

  <div style='max-width:600px; margin:auto; background:#ffffff; border-radius:10px; overflow:hidden; box-shadow:0 4px 12px rgba(0,0,0,0.15);'>

    <!-- HEADER -->
    <div style='background:{headerColor}; color:#ffffff; padding:18px; text-align:center; font-size:22px; font-weight:bold;'>
      Payslip Request {statusText}
    </div>

    <!-- BODY -->
    <div style='padding:25px; color:#333; font-size:15px;'>

      <p>Dear {employeeName},</p>

      <p>
        Your payslip request has been <b>{statusText}</b>.
      </p>

      <!-- TABLE -->
      <table style='width:100%; border-collapse:collapse; margin-top:15px; font-size:14px;'>

        <tr>
          <td style='padding:10px; border:1px solid #ddd; background:#f8f9fa; font-weight:bold;'>
            Employee Name
          </td>
          <td style='padding:10px; border:1px solid #ddd;'>
            {employeeName}
          </td>
        </tr>

        <tr>
          <td style='padding:10px; border:1px solid #ddd; background:#f8f9fa; font-weight:bold;'>
            Month
          </td>
          <td style='padding:10px; border:1px solid #ddd;'>
            {monthDisplay}
          </td>
        </tr>

        <tr>
          <td style='padding:10px; border:1px solid #ddd; background:#f8f9fa; font-weight:bold;'>
            Year
          </td>
          <td style='padding:10px; border:1px solid #ddd;'>
            {year}
          </td>
        </tr>

        <tr>
          <td style='padding:10px; border:1px solid #ddd; background:#f8f9fa; font-weight:bold;'>
            Total Payslips
          </td>
          <td style='padding:10px; border:1px solid #ddd;'>
            {dto.PayrollIds.Count}
          </td>
        </tr>

        <tr>
          <td style='padding:10px; border:1px solid #ddd; background:#f8f9fa; font-weight:bold;'>
            Status
          </td>
          <td style='padding:10px; border:1px solid #ddd; font-weight:bold; color:{headerColor};'>
            {statusText}
          </td>
        </tr>

      </table>

      <p style='margin-top:20px;'>
        You can now proceed accordingly.
      </p>

    </div>

    <!-- FOOTER -->
    <div style='background:#f1f1f1; padding:12px; text-align:center; font-size:12px; color:#777;'>
      © {DateTime.Now.Year} Cortracker360 HRMS System
    </div>

  </div>

</div>";

            // 🔥 SEND EMAIL
            if (!string.IsNullOrEmpty(employeeEmail))
            {
                await _emailService.SendEmailAsync(
                    employeeEmail,
                    $"Payslip Request {statusText}",
                    body,
                    null
                );
            }
        }

        /* ============================================================
           Get All Payrolls By selected months Range
        ============================================================ */
        public async Task<List<PayrollTransactionDto>> GetAllPayrolls(HrPayrollFilterDto dto)
        {
            var query = _context.PayrollTransactions
                .Where(x =>
                    x.CompanyId == dto.CompanyId.ToString() &&
                    x.RegionId == dto.RegionId.ToString() &&
                    x.Year == dto.Year &&
                    x.Month >= dto.FromMonth &&
                    x.Month <= dto.ToMonth
                );

            if (dto.EmployeeId.HasValue)
                query = query.Where(x => x.EmployeeId == dto.EmployeeId);

            var transactions = await query.ToListAsync();

            // 🔥 MASTER DATA (LOAD ONCE)
            var users = await _context.Users.ToListAsync();

            var departments = await _context.Departments
                .Where(d => d.CompanyId == dto.CompanyId && d.RegionId == dto.RegionId)
                .ToListAsync();

            var designations = await _context.Designations
      .Where(d => d.CompanyId == dto.CompanyId && d.RegionId == dto.RegionId)
      .ToListAsync();

            var personalDetails = await _context.EmployeePersonalDetails
                .Where(p => p.CompanyId == dto.CompanyId && p.RegionId == dto.RegionId)
                .ToListAsync();

            var result = new List<PayrollTransactionDto>();

            foreach (var trx in transactions)
            {
                // 🔥 USER
                var user = users.FirstOrDefault(u => u.UserId == trx.EmployeeId);

                // 🔥 BANK DETAILS (FIXED USING USERID)
                var bank = await _context.EmployeeBankDetails
                    .Where(b =>
                        b.UserId == trx.EmployeeId &&
                        b.CompanyId == dto.CompanyId &&
                        b.RegionId == dto.RegionId
                    )
                    .OrderByDescending(b => b.CreatedAt)
                    .FirstOrDefaultAsync();

                // 🔥 PERSONAL DETAILS (DATE OF JOINING)
                var personal = personalDetails
                    .FirstOrDefault(p =>
                        p.UserId == trx.EmployeeId &&
                        p.CompanyId == dto.CompanyId &&
                        p.RegionId == dto.RegionId
                    );

                // 🔥 DEPARTMENT NAME
                var departmentName = departments
                    .FirstOrDefault(d => d.DepartmentId == user?.DepartmentId)
                    ?.DepartmentName;

                var designationName = designations
          .FirstOrDefault(d => d.DesignationId == user?.DesignationId)
          ?.DesignationName;

                // 🔥 DESIGNATION (STRING DIRECT)
                //  var designationName = user?.Designation;

                // 🔥 PAYROLL DETAILS (EARNINGS + DEDUCTIONS)
                var details = await _context.PayrollDetails
                    .Where(d => d.PayrollId == trx.PayrollId)
                    .ToListAsync();

                var componentIds = details.Select(d => d.ComponentId).ToList();

                var components = await _context.SalaryComponents
                    .Where(c => componentIds.Contains(c.ComponentId))
                    .ToListAsync();

                var detailDtos = details.Select(d => new PayrollDetailDto
                {
                    ComponentId = d.ComponentId,
                    Amount = d.Amount,
                    ComponentName = components
                        .FirstOrDefault(c => c.ComponentId == d.ComponentId)?.ComponentName ?? "Other",
                    Type = components
                        .FirstOrDefault(c => c.ComponentId == d.ComponentId)?.Type ?? "Other"
                }).ToList();

                // 🔥 FINAL RESULT
                result.Add(new PayrollTransactionDto
                {
                    PayrollId = trx.PayrollId,
                    EmployeeId = trx.EmployeeId,
                    Month = trx.Month,
                    Year = trx.Year,

                    GrossSalary = trx.GrossSalary,
                    TotalDeductions = trx.TotalDeductions,
                    NetSalary = trx.NetSalary,

                    // ✅ USER DETAILS
                    EmployeeName = user?.FullName,
                    Designation = designationName ?? "-",
                    Department = departmentName,
                    Location = "Hyderabad",

                    // ✅ DATE OF JOINING FROM PERSONAL TABLE
                    JoiningDate = personal?.DateOfJoining,

                    EmployeeCode = user?.EmployeeCode,

                    // ✅ BANK DETAILS
                    Bank = bank?.BankName,
                    AccountNo = bank?.AccountNumber,

                    // ✅ PAN FROM PERSONAL TABLE
                    Pan = personal?.Pannumber ?? "-",

                    // ✅ DETAILS (EARNINGS + DEDUCTIONS)
                    Details = detailDtos
                });
            }

            return result;
        }

        // ================= GET LATE LOGIN POLICY =================
        private async Task<LateLoginPolicy?> GetLateLoginPolicy(int userId, int companyId, int regionId)
        {
            return await _context.LateLoginPolicies
                .Where(x =>
                    x.UserId == userId &&
                    x.CompanyId == companyId &&
                    x.RegionId == regionId &&
                    x.IsActive == true   // ✅ FIXED
                )
                .OrderByDescending(x => x.ModifiedAt ?? x.CreatedAt)
                .FirstOrDefaultAsync();
        }
    }
}
