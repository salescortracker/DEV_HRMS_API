using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using DataAccessLayer.Repositories.GeneralRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Implementations
{
    public class AttendanceService : IAttendanceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly HRMSContext _hrmsContext;
        private readonly IEmailService _emailService;

        public AttendanceService(IUnitOfWork unitOfWork,HRMSContext hRMSContext, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _hrmsContext = hRMSContext;
            _emailService = emailService;
        }

        // ================================
        // GET TODAY EMPLOYEES
        // ================================
        public async Task<List<EmployeeAttendanceDto>> GetTodayEmployees(int companyId, int regionId)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            var users = (await _unitOfWork.Repository<User>().GetAllAsync())
                .Where(e => e.CompanyId == companyId
                         && e.RegionId == regionId
                         && !string.IsNullOrEmpty(e.EmployeeCode))
                .ToList();

            // ✅ ADD THIS BLOCK HERE
            if (IsWeekend(today))
            {
                return users.Select(emp => new EmployeeAttendanceDto
                {
                    EmployeeCode = emp.EmployeeCode,
                    EmployeeName = emp.FullName,
                    AttendanceDate = DateTime.Today,
                    Status = "WeekOff",
                    ClockIn = null,
                    ClockOut = null,
                    GrossTime = null,
                    ShiftName = "",
                    ShiftStartTime = "",
                    ShiftEndTime = "",
                    LateMinutes = 0
                }).ToList();
            }

            var clockRecords = await _unitOfWork.Repository<ClockInOut>().GetAllAsync();
            var leaves = await _unitOfWork.Repository<LeaveRequest>().GetAllAsync();
            var shiftAllocations = await _unitOfWork.Repository<ShiftAllocation>().GetAllAsync();
            var shiftMasters = await _unitOfWork.Repository<ShiftMaster>().GetAllAsync();
            var leaveTypes = await _unitOfWork.Repository<LeaveType>().GetAllAsync();

            var result = new List<EmployeeAttendanceDto>();

            foreach (var emp in users)
            {
                string status = "Absent";
                string clockInTime = null;
                string clockOutTime = null;
                string grossTime = null;
                int? lateMinutes = null;
                string arrivalStatus = "";

                // ===== GET SHIFT =====
                var shiftAlloc = shiftAllocations
                    .FirstOrDefault(s => s.EmployeeCode == emp.EmployeeCode && s.IsActive);

                var shiftMaster = shiftMasters
                    .Where(sm => sm.CompanyId == companyId && sm.RegionId == regionId).FirstOrDefault();

                TimeOnly? shiftStart = shiftMaster.ShiftStartTime;
                TimeOnly? shiftEnd = shiftMaster?.ShiftEndTime;
                string shiftName = shiftMaster?.ShiftName;
                TimeOnly? graceTimeValue = shiftMaster?.GraceTime;

                // ================= LEAVE CHECK =================
                var leave = leaves.FirstOrDefault(l =>
                    l.UserId == emp.UserId &&
                    l.Status == "Approved" &&
                    l.StartDate <= today &&
                    l.EndDate >= today);

                if (leave != null)
                {
                    var leaveType = leaveTypes
                        .FirstOrDefault(t => t.LeaveTypeId == leave.LeaveTypeId);

                    status = leaveType?.LeaveTypeName ?? "Leave";
                }
                else
                {
                    var records = clockRecords
                        .Where(c =>
                            c.EmployeeCode == emp.EmployeeCode &&
                            c.CompanyId == companyId &&
                            c.RegionId == regionId &&
                            c.AttendanceDate == today)
                        .OrderBy(c => c.ActionTime)
                        .ToList();

                    var clockIn = records
                        .FirstOrDefault(r => r.ActionType == "ClockIn")?.ActionTime;

                    var clockOut = records
                        .LastOrDefault(r => r.ActionType == "ClockOut")?.ActionTime;

                    if (clockIn != null)
                        clockInTime = clockIn.Value.ToString("HH:mm");

                    if (clockOut != null)
                        clockOutTime = clockOut.Value.ToString("HH:mm");

                    // ===== GROSS TIME =====
                    // If ClockIn exists → at least Present
                    if (clockIn != null)
                    {
                        status = "Present";
                    }

                    // If both exist → calculate properly
                    if (clockIn != null && clockOut != null)
                    {
                        var duration = clockOut.Value - clockIn.Value;
                        grossTime = duration.ToString(@"hh\:mm");

                        if (duration.TotalHours >= 5)
                            status = "Present";
                        else
                            status = "HalfDay";
                    }


                    // ===== LATE MINUTES =====
                    if (clockIn != null && shiftStart.HasValue)
                    {
                        var clockInTimeOnly = clockIn.Value;

                        // ✅ Convert GraceTime properly
                        int graceMinutes = 0;

                        if (graceTimeValue.HasValue)
                        {
                            graceMinutes = (graceTimeValue.Value.Hour * 60)
                                         + graceTimeValue.Value.Minute;
                        }

                        // ✅ Apply grace time to shift start
                        var allowedTime = shiftStart.Value.AddMinutes(graceMinutes);

                        // ✅ ONLY calculate after grace
                        if (clockInTimeOnly > allowedTime)
                        {
                            lateMinutes = (int)(clockInTimeOnly - allowedTime).TotalMinutes;
                        }
                        else
                        {
                            lateMinutes = 0;
                        }
                    }
                }

                result.Add(new EmployeeAttendanceDto
                {
                    EmployeeCode = emp.EmployeeCode,
                    EmployeeName = emp.FullName,
                    AttendanceDate = DateTime.Today,
                    Status = status,
                    ClockIn = clockInTime,
                    ClockOut = clockOutTime,
                    GrossTime = grossTime,

                    // ✅ IMPORTANT ADD THESE
                    ShiftName = shiftName,
                    ShiftStartTime = shiftStart?.ToString("HH:mm"),
                    ShiftEndTime = shiftEnd?.ToString("HH:mm"),
                    LateMinutes = lateMinutes,
                    //GraceTime = graceTimeValue?.ToString("HH:mm")
                });
            }

            return result;
        }
        // ================================
        // SAVE ATTENDANCE
        // ================================
        public async Task SaveAttendanceAsync(SaveAttendanceDto dto, int userId)
        {
            var repo = _unitOfWork.Repository<EmployeeAttendance>();

            var attendanceDate = DateOnly.FromDateTime(dto.AttendanceDate);

            if (IsWeekend(attendanceDate))
            {
                throw new Exception("Cannot save attendance for WeekOff (Saturday/Sunday)");
            }

            var shiftMasters = await _unitOfWork.Repository<ShiftMaster>().GetAllAsync();

            //var existingRecords = (await repo.GetAllAsync())
            //    .Where(x => x.CompanyId == dto.CompanyId &&
            //                x.RegionId == dto.RegionId &&
            //                x.AttendanceDate == attendanceDate)
            //    .ToList();
            var existingRecords = _hrmsContext.EmployeeAttendances
                .Where(x => x.CompanyId == dto.CompanyId &&
                            x.RegionId == dto.RegionId &&
                            x.AttendanceDate == attendanceDate)
                .ToList();

            foreach (var emp in dto.Employees)
            {
                // ✅ SAME LOGIC AS GET METHOD
                var shiftMaster = shiftMasters
                    .Where(sm => sm.CompanyId == dto.CompanyId && sm.RegionId == dto.RegionId)
                    .FirstOrDefault();

                TimeOnly? shiftStart = shiftMaster?.ShiftStartTime;
                TimeOnly? shiftEnd = shiftMaster?.ShiftEndTime;
                string shiftName = shiftMaster?.ShiftName;
                TimeOnly? graceTimeValue = shiftMaster?.GraceTime;

                int? lateMinutes = null;

                // ✅ FIXED LATE CALCULATION
                if (!string.IsNullOrEmpty(emp.ClockIn) && shiftStart.HasValue)
                {
                    var clockIn = TimeOnly.Parse(emp.ClockIn);

                    int graceMinutes = 0;

                    if (graceTimeValue.HasValue)
                    {
                        graceMinutes = (graceTimeValue.Value.Hour * 60)
                                     + graceTimeValue.Value.Minute;
                    }

                    var allowedTime = shiftStart.Value.AddMinutes(graceMinutes);

                    if (clockIn > allowedTime)
                    {
                        lateMinutes = (int)(
                            clockIn.ToTimeSpan() - allowedTime.ToTimeSpan()
                        ).TotalMinutes;
                    }
                    else
                    {
                        lateMinutes = 0;
                    }
                }

                var existing = existingRecords
                    .FirstOrDefault(x => x.EmployeeCode == emp.EmployeeCode);

                if (existing != null)
                {
                    // UPDATE
                    existing.Status = emp.Status;

                    existing.ClockInTime = string.IsNullOrEmpty(emp.ClockIn)
                        ? null
                        : TimeOnly.Parse(emp.ClockIn);

                    existing.ClockOutTime = string.IsNullOrEmpty(emp.ClockOut)
                        ? null
                        : TimeOnly.Parse(emp.ClockOut);

                    existing.GrossTime = emp.GrossTime;

                    existing.ModifiedBy = userId.ToString();
                    existing.ModifiedAt = DateTime.Now;

                    existing.ShiftName = shiftName;
                    existing.ShiftStartTime = shiftStart;
                    existing.ShiftEndTime = shiftEnd;
                    existing.LateMinutes = lateMinutes;
                }
                else
                {
                    // INSERT
                    var entity = new EmployeeAttendance
                    {
                        RegionId = dto.RegionId,
                        CompanyId = dto.CompanyId,
                        EmployeeCode = emp.EmployeeCode,
                        EmployeeName = emp.EmployeeName,
                        AttendanceDate = attendanceDate,
                        Status = emp.Status,

                        ClockInTime = string.IsNullOrEmpty(emp.ClockIn)
                            ? null
                            : TimeOnly.Parse(emp.ClockIn),

                        ClockOutTime = string.IsNullOrEmpty(emp.ClockOut)
                            ? null
                            : TimeOnly.Parse(emp.ClockOut),

                        GrossTime = emp.GrossTime,

                        ShiftName = shiftName,
                        ShiftStartTime = shiftStart,
                        ShiftEndTime = shiftEnd,
                        LateMinutes = lateMinutes,

                        CreatedBy = userId,
                        CreatedAt = DateTime.Now,
                    };

                    await repo.AddAsync(entity);
                }
            }

            await _unitOfWork.CompleteAsync();
        }

        // ================================
        // DATES RANGE REPORT
        // ================================
        public async Task<List<EmployeeAttendanceDto>> GetDateRangeReport(
    int companyId,
    int regionId,
    DateTime fromDate,
    DateTime toDate)
        {
            // var data = await _unitOfWork.Repository<EmployeeAttendance>().GetAllAsync();
            var data = _hrmsContext.EmployeeAttendances.Select(y => new { y.CompanyId,y.RegionId,y.AttendanceDate,y.ShiftName,y.ShiftStartTime,y.ShiftEndTime,y.EmployeeCode,y.EmployeeName,y.ClockInTime,y.ClockOutTime,y.GrossTime,y.Status,y.LateMinutes}).ToList();
            var startDate = DateOnly.FromDateTime(fromDate);
            var endDate = DateOnly.FromDateTime(toDate);

            return data
                .Where(x =>
                    x.CompanyId == companyId &&
                    x.RegionId == regionId &&
                    x.AttendanceDate.HasValue &&
                    x.AttendanceDate.Value >= startDate &&
                    x.AttendanceDate.Value <= endDate)
                .Select(x =>
                {
                    EmployeeAttendance employeeAttendance = new EmployeeAttendance
                    {
                        CompanyId = x.CompanyId,
                        RegionId = x.RegionId,
                        AttendanceDate = x.AttendanceDate,
                        EmployeeCode=x.EmployeeCode,
                        EmployeeName=x.EmployeeName,
                        LateMinutes=x.LateMinutes,
                        Status=x.Status,
                        ClockInTime=x.ClockInTime,
                        ClockOutTime=x.ClockOutTime,
                        GrossTime=x.GrossTime,
                        ShiftStartTime=x.ShiftStartTime,
                        ShiftEndTime=x.ShiftEndTime,
                        ShiftName=x.ShiftName
                       
                    };
                    var dto = MapToDto(employeeAttendance);

                    if (IsWeekend(x.AttendanceDate.Value))
                    {
                        dto.Status = "WeekOff";
                    }

                    return dto;
                })
                .OrderByDescending(x => x.AttendanceDate)
                .ToList();
        }

        // ================================
        // WEEKLY REPORT
        // ================================
        public async Task<List<EmployeeAttendanceDto>> GetWeeklyReport(int companyId, int regionId)
        {
            var data = await _unitOfWork.Repository<EmployeeAttendance>().GetAllAsync();

            var startDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-7));
            var endDate = DateOnly.FromDateTime(DateTime.Today);

            return data
                .Where(x =>
                    x.CompanyId == companyId &&
                    x.RegionId == regionId &&
                    x.AttendanceDate.HasValue &&
                    x.AttendanceDate.Value >= startDate &&
                    x.AttendanceDate.Value <= endDate)
                .OrderByDescending(x => x.AttendanceDate)
                .Select(x =>
                {
                    var dto = MapToDto(x);

                    if (x.AttendanceDate.HasValue && IsWeekend(x.AttendanceDate.Value))
                    {
                        dto.Status = "WeekOff";
                    }

                    return dto;
                })
                .ToList();
        }

        // ================================
        // MONTHLY REPORT
        // ================================
        public async Task<List<EmployeeAttendanceDto>> GetMonthlyReport(int companyId, int regionId)
        {
            var data = await _unitOfWork.Repository<EmployeeAttendance>().GetAllAsync();

            var today = DateTime.Today;

            return data
                .Where(x =>
                    x.CompanyId == companyId &&
                    x.RegionId == regionId &&
                    x.AttendanceDate.HasValue &&
                    x.AttendanceDate.Value.Month == today.Month &&
                    x.AttendanceDate.Value.Year == today.Year)
                .OrderByDescending(x => x.AttendanceDate)
                .Select(x =>
                {
                    var dto = MapToDto(x);

                    if (x.AttendanceDate.HasValue && IsWeekend(x.AttendanceDate.Value))
                    {
                        dto.Status = "WeekOff";
                    }

                    return dto;
                })
                .ToList();
        }

        // ================================
        // MAP ENTITY → DTO
        // ================================
        private static EmployeeAttendanceDto MapToDto(EmployeeAttendance entity)
        {
            return new EmployeeAttendanceDto
            {
                AttendanceId = entity.AttendanceId,

                RegionId = entity.RegionId ?? 0,
                CompanyId = entity.CompanyId ?? 0,

                EmployeeCode = entity.EmployeeCode ?? "",
                EmployeeName = entity.EmployeeName ?? "",

                AttendanceDate = entity.AttendanceDate.HasValue
                    ? entity.AttendanceDate.Value.ToDateTime(TimeOnly.MinValue)
                    : DateTime.MinValue,

                Status = entity.Status ?? "",

                ClockIn = entity.ClockInTime?.ToString("HH:mm"),
                ClockOut = entity.ClockOutTime?.ToString("HH:mm"),
                GrossTime = entity.GrossTime,
                ShiftName = entity.ShiftName,
                //GraceTime = entity.GraceTime,

                ShiftStartTime = entity.ShiftStartTime?.ToString("HH:mm"),

                ShiftEndTime = entity.ShiftEndTime?.ToString("HH:mm"),

                LateMinutes = entity.LateMinutes,
            };
        }

        // ================================
        // GetEmployeesByDate
        // ================================

        public async Task<List<EmployeeAttendanceDto>> GetEmployeesByDate(int companyId, int regionId, DateTime date)
        {
            var selectedDate = DateOnly.FromDateTime(date);

            var users = (await _unitOfWork.Repository<User>().GetAllAsync())
                .Where(e => e.CompanyId == companyId
                         && e.RegionId == regionId
                         && !string.IsNullOrEmpty(e.EmployeeCode))
                .ToList();

            var clockRecords = await _unitOfWork.Repository<ClockInOut>().GetAllAsync();
            var leaves = await _unitOfWork.Repository<LeaveRequest>().GetAllAsync();
            var shiftMasters = await _unitOfWork.Repository<ShiftMaster>().GetAllAsync();
            var leaveTypes = await _unitOfWork.Repository<LeaveType>().GetAllAsync();
            var workFromHomes = await _unitOfWork.Repository<WfhremoteRequest>().GetAllAsync();

            var result = new List<EmployeeAttendanceDto>();

            foreach (var emp in users)
            {
                if (IsWeekend(selectedDate))
                {
                    result.Add(new EmployeeAttendanceDto
                    {
                        EmployeeCode = emp.EmployeeCode,
                        EmployeeName = emp.FullName,
                        AttendanceDate = date,
                        Status = "WeekOff",
                        ClockIn = null,
                        ClockOut = null,
                        GrossTime = null,
                        ShiftName = "",
                        ShiftStartTime = "",
                        ShiftEndTime = "",
                        LateMinutes = 0,

                    });

                    continue;
                }
                string status = "Absent";
                string clockInTime = null;
                string clockOutTime = null;
                string grossTime = null;
                int? lateMinutes = null;
                string arrivalStatus = "";

                // ✅ GET SHIFT
                var shiftMaster = shiftMasters
                    .FirstOrDefault(sm => sm.CompanyId == companyId && sm.RegionId == regionId);

                TimeOnly? shiftStart = shiftMaster?.ShiftStartTime;
                TimeOnly? shiftEnd = shiftMaster?.ShiftEndTime;
                string shiftName = shiftMaster?.ShiftName;
                TimeOnly? graceTimeValue = shiftMaster?.GraceTime;

                // ================= LEAVE CHECK =================
                var leave = leaves.FirstOrDefault(l =>
                    l.UserId == emp.UserId &&
                    l.Status == "Approved" &&
                    l.StartDate <= selectedDate &&
                    l.EndDate >= selectedDate);

                if (leave != null)
                {
                    var leaveType = leaveTypes
                        .FirstOrDefault(t => t.LeaveTypeId == leave.LeaveTypeId);

                    status = leaveType?.LeaveTypeName ?? "Leave";
                }
                else
                {
                    var records = clockRecords
                        .Where(c =>
                            c.EmployeeCode == emp.EmployeeCode &&
                            c.CompanyId == companyId &&
                            c.RegionId == regionId &&
                            c.AttendanceDate == selectedDate)
                        .OrderBy(c => c.ActionTime)
                        .ToList();

                    var clockIn = records.FirstOrDefault(r => r.ActionType == "ClockIn")?.ActionTime;
                    var clockOut = records.LastOrDefault(r => r.ActionType == "ClockOut")?.ActionTime;

                    if (clockIn != null)
                    {
                        clockInTime = clockIn.Value.ToString("HH:mm");
                        status = "Present";
                    }

                    if (clockOut != null)
                    {
                        clockOutTime = clockOut.Value.ToString("HH:mm");
                    }

                    if (
                          clockIn != null &&
                         clockOut == null
                        )
                    {
                        if (
                            selectedDate < DateOnly.FromDateTime(DateTime.Today)
                        )
                        {
                            status = "Incomplete Attendance";
                        }
                        else if (
                            shiftEnd.HasValue &&
                            TimeOnly.FromDateTime(DateTime.Now) > shiftEnd.Value
                        )
                        {
                            status = "Incomplete Attendance";
                        }
                    }

                    // ✅ GROSS TIME
                    if (clockIn != null && clockOut != null)
                    {
                        var duration = clockOut.Value - clockIn.Value;
                        grossTime = duration.ToString(@"hh\:mm");

                        status = duration.TotalHours >= 5 ? "Present" : "HalfDay";
                    }

                    // ✅ ✅ FIXED LATE LOGIC (IMPORTANT)

                    if (clockIn != null && shiftStart.HasValue)
                    {
                        int graceMinutes = 0;

                        if (graceTimeValue.HasValue)
                        {
                            graceMinutes =
                                (graceTimeValue.Value.Hour * 60)
                                + graceTimeValue.Value.Minute;
                        }

                        var allowedTime = shiftStart.Value.AddMinutes(graceMinutes);

                        // =========================
                        // EARLY LOGIN
                        // =========================
                        if (clockIn.Value < shiftStart.Value)
                        {
                            var earlyMinutes =
                                (int)(shiftStart.Value - clockIn.Value).TotalMinutes;

                            arrivalStatus = $"Early by {earlyMinutes} mins";

                            lateMinutes = 0;
                        }

                        // =========================
                        // EXACT SHIFT TIME
                        // =========================
                        else if (clockIn.Value == shiftStart.Value)
                        {
                            arrivalStatus = "On Time";

                            lateMinutes = 0;
                        }

                        // =========================
                        // WITHIN GRACE TIME
                        // =========================
                        else if (clockIn.Value <= allowedTime)
                        {
                            int graceUsed =
                                (int)(clockIn.Value - shiftStart.Value).TotalMinutes;

                            arrivalStatus =
                                $"Within Grace Time ({graceUsed} mins)";

                            lateMinutes = 0;
                        }

                        // =========================
                        // LATE LOGIN
                        // =========================
                        else
                        {
                            lateMinutes =
                                (int)(clockIn.Value - allowedTime).TotalMinutes;

                            arrivalStatus =
                                $"Late by {lateMinutes} mins";
                        }
                    }
                }

                result.Add(new EmployeeAttendanceDto
                {
                    EmployeeCode = emp.EmployeeCode,
                    EmployeeName = emp.FullName +
    (workFromHomes.Any(w =>
        w.EmployeeId == emp.UserId &&
        w.CompanyId == companyId &&
        w.RegionId == regionId &&
        w.Status == "Approved" &&
        w.FromDate <= selectedDate &&
        w.ToDate >= selectedDate
    ) ? " (WFH)" : ""),
                    AttendanceDate = date,
                    Status = status,
                    ClockIn = clockInTime,
                    ClockOut = clockOutTime,
                    GrossTime = grossTime,

                    // ✅ IMPORTANT RETURN THESE
                    ShiftName = shiftName,
                    ShiftStartTime = shiftStart?.ToString("HH:mm"),
                    ShiftEndTime = shiftEnd?.ToString("HH:mm"),
                    LateMinutes = lateMinutes,
                    ArrivalStatus = arrivalStatus
                });
            }

            return result;
        }

        // ================================
        // GetUnsavedDates
        // ================================
        public async Task<List<DateTime>> GetUnsavedDates(int companyId, int regionId)
        {
            var attendanceData = await _unitOfWork.Repository<EmployeeAttendance>().GetAllAsync();

            var last7Days = Enumerable.Range(0, 7)
                .Select(d => DateTime.Today.AddDays(-d).Date)
                .Where(d => d.DayOfWeek != DayOfWeek.Saturday &&
                            d.DayOfWeek != DayOfWeek.Sunday)
                .ToList();

            var savedDates = attendanceData
                .Where(x => x.CompanyId == companyId && x.RegionId == regionId)
                .Select(x => x.AttendanceDate.Value.ToDateTime(TimeOnly.MinValue).Date)
                .Distinct()
                .ToList();

            var unsavedDates = last7Days
                .Where(d => !savedDates.Contains(d))
                .ToList();

            return unsavedDates;
        }


        private bool IsWeekend(DateOnly date)
        {
            return date.DayOfWeek == DayOfWeek.Saturday ||
                   date.DayOfWeek == DayOfWeek.Sunday;
        }
        public async Task ProcessClockOutReminders()
        {
            var now = DateTime.Now;

            // STEP 1: Latest active clockin per user
            var pendingEmployees = await _hrmsContext.ClockInOuts
                .Where(x =>
                    x.ClockInTime != null &&
                    x.ClockOutTime == null)// &&
                    //x.ShiftEndReminderSent < 2)
                .GroupBy(x => x.CreatedBy)
                .Select(g => g
                    .OrderByDescending(x => x.ClockInTime)
                    .First())
                .ToListAsync();

            foreach (var emp in pendingEmployees)
            {
                // ✅ FIX NULL ISSUE
                if (!emp.CreatedBy.HasValue)
                    continue;

                int userId = emp.CreatedBy.Value;

                // STEP 2: Shift Allocation
                var shiftAlloc = await _hrmsContext.ShiftAllocations
                    .FirstOrDefaultAsync(x =>
                        x.UserId == userId &&
                        x.IsActive);

                if (shiftAlloc == null)
                    continue;

                // STEP 3: Shift Master
                var shift = await _hrmsContext.ShiftMasters
                    .FirstOrDefaultAsync(x =>
                        x.ShiftId == shiftAlloc.ShiftId);

                if (shift == null)
                    continue;

                // STEP 4: Shift End Time
                var shiftEnd = emp.AttendanceDate.ToDateTime(shift.ShiftEndTime);

                if (shift.ShiftEndTime < shift.ShiftStartTime)
                    shiftEnd = shiftEnd.AddDays(1);

                var firstReminder = shiftEnd.AddMinutes(5);
                var secondReminder = shiftEnd.AddMinutes(10);

                // STEP 5: User
                var user = await _hrmsContext.Users
                    .FirstOrDefaultAsync(x => x.UserId == userId);

                if (user == null || string.IsNullOrWhiteSpace(user.Email))
                    continue;
                var dbRow = await _hrmsContext.ClockInOuts
                    .FirstOrDefaultAsync(x =>
                        x.ClockInOutId == emp.ClockInOutId);

                if (dbRow == null || dbRow.ShiftEndReminderSent >= 2)
                    continue;

                // FIRST REMINDER
                if (dbRow.ShiftEndReminderSent == 0 && now >= firstReminder)
                {
                    await SendMail(userId, dbRow.EmployeeCode, 1);

                    dbRow.ShiftEndReminderSent = 1;

                    await _hrmsContext.SaveChangesAsync();
                }

                else if (dbRow.ShiftEndReminderSent == 1 && now >= secondReminder)
                {
                    // 1️⃣ LOCK FIRST
                    dbRow.ShiftEndReminderSent = 2;
                    await _hrmsContext.SaveChangesAsync();

                    // 2️⃣ SEND FINAL EMAIL
                    await SendMail(userId, dbRow.EmployeeCode, 2);

                }
            }

            await _hrmsContext.SaveChangesAsync();
        }

        private async Task SendMail(int? userId, string employeeCode, int reminderType)
        {
            if (!userId.HasValue)
                return;

            var user = await _hrmsContext.Users
                .FirstOrDefaultAsync(x => x.UserId == userId.Value);

            if (user == null || string.IsNullOrWhiteSpace(user.Email))
                return;

            string employeeName = user.FullName ?? "Employee";

            string subject = "Clock Out Reminder";

            string message = reminderType == 1
                ? $@"
                    <html>
                    <body>
                    <p>Dear {employeeName},</p>

                    <p>This is a reminder that your shift has ended but your clock-out entry is still pending.</p>

                    <p>
                    <b>Employee Code:</b> {employeeCode}<br/>
                    <b>Shift Status:</b> First Reminder
                    </p>

                    <p>Please complete your clock-out in the HRMS portal.</p>

                    <p>Regards,<br/>HRMS Team</p>
                    </body>
                    </html>"
                                    : $@"
                    <html>
                    <body>
                    <p>Dear {employeeName},</p>

                    <p>This is a <b>FINAL</b> reminder that your shift has ended and clock-out is still pending.</p>

                    <p>
                    <b>Employee Code:</b> {employeeCode}<br/>
                    <b>Shift Status:</b> Final Reminder
                    </p>

                    <p style='color:#d9534f;font-weight:bold;margin-top:15px;'>
                    👉 Please go to <b>Missed Punch Request</b> and submit your details immediately.
                    </p>

                    <p>Regards,<br/>HRMS Team</p>
                    </body>
                    </html>";

            try
            {
                await _emailService.SendEmailAsync(user.Email, subject, message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email failed: {ex.Message}");
            }
        }
    }
}