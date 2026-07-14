using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using DataAccessLayer.Repositories.GeneralRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Globalization;

namespace BusinessLayer.Implementations
{
    public class ClockInOutService : IClockInOutService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly HRMSContext _context;
        private readonly INotificationService _notificationService;
        public ClockInOutService(IUnitOfWork unitOfWork, IEmailService emailService, HRMSContext context, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _context = context;
            _notificationService = notificationService;
        }

        public async Task<IEnumerable<ClockInOutDto>> GetAllAsync()
        {
            var data = await _unitOfWork.Repository<ClockInOut>().GetAllAsync();

            return data
                .OrderByDescending(x => x.AttendanceDate)   
                .ThenByDescending(x => x.ActionTime)
                .Select(MapToDto)
                .ToList();
        }


        public async Task<ClockInOutDto?> GetByIdAsync(int id)
        {
            var entity = await _unitOfWork.Repository<ClockInOut>().GetByIdAsync(id);
            return entity == null ? null : MapToDto(entity);
        }

        public async Task<IEnumerable<ClockInOutDto>> GetTodayByEmployeeAsync(
            string employeeCode, int companyId, int regionId)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var data = await _unitOfWork.Repository<ClockInOut>().GetAllAsync();

            return data
                .Where(x =>
                    x.EmployeeCode == employeeCode &&
                    x.CompanyId == companyId &&
                    x.RegionId == regionId &&
                    x.AttendanceDate == today)
                .OrderBy(x => x.ActionTime)
                .Select(MapToDto)
                .ToList();
        }

        public async Task<IEnumerable<ClockInOutDto>>
GetAttendanceByDateRangeAsync(
    string employeeCode,
    int companyId,
    int regionId,
    DateOnly fromDate,
    DateOnly toDate)
        {
            var data =
                await _unitOfWork.Repository<ClockInOut>()
                .GetAllAsync();

            return data
                .Where(x =>
                    x.EmployeeCode == employeeCode &&
                    x.CompanyId == companyId &&
                    x.RegionId == regionId &&
                    x.AttendanceDate >= fromDate &&
                    x.AttendanceDate <= toDate)
                .OrderByDescending(x => x.AttendanceDate)
                .ThenBy(x => x.ActionTime)
                .Select(MapToDto)
                .ToList();
        }

        //public async Task<ClockInOutDto> AddAsync(ClockInOutCreateDto dto, int userId)
        //{
        //    try
        //    {
        //        var entity = new ClockInOut
        //        {
        //            RegionId = dto.RegionId,
        //            CompanyId = dto.CompanyId,
        //            EmployeeCode = dto.EmployeeCode,
        //            EmployeeName = dto.EmployeeName,
        //            Department = dto.Department,
        //            ClockInTime = string.IsNullOrWhiteSpace(dto.clockInTime)    ? null
        //                          : TimeOnly.ParseExact(dto.clockInTime, "HH:mm", CultureInfo.InvariantCulture),
        //            ClockOutTime = string.IsNullOrWhiteSpace(dto.clockOutTime)
        //                            ? null
        //                            : TimeOnly.ParseExact(dto.clockOutTime, "HH:mm", CultureInfo.InvariantCulture),
        //            ActionTime =  TimeOnly.ParseExact(dto.ActionTime, "HH:mm", CultureInfo.InvariantCulture),
        //            AttendanceDate = DateOnly.FromDateTime(dto.AttendanceDate),
        //            ActionType = dto.ActionType,                 // ClockIn / ClockOut
        //                                                         //ActionTime = DateTime.Now.TimeOfDay,          // ✅ FIXED
        //            Status = dto.ActionType == "ClockIn"
        //                        ? "Present"
        //                        : "Completed",
        //            CreatedBy = userId,
        //            CreatedAt = DateTime.Now
        //        };

        //        await _unitOfWork.Repository<ClockInOut>().AddAsync(entity);
        //        await _unitOfWork.CompleteAsync();

        //        return MapToDto(entity);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Error adding ClockInOut record: " + ex.Message);
        //    }
        //}

        public async Task<ClockInOutDto> AddAsync(ClockInOutCreateDto dto, int userId)

        {
            try
            {
                var entity = new ClockInOut
                {
                    RegionId = dto.RegionId,

                    CompanyId = dto.CompanyId,

                    EmployeeCode = dto.EmployeeCode,

                    EmployeeName = dto.EmployeeName,

                    Department = dto.Department,

                    ClockInTime =
                        string.IsNullOrWhiteSpace(dto.clockInTime)
                        ? null
                        : TimeOnly.ParseExact(
                            dto.clockInTime,
                            "HH:mm",
                            CultureInfo.InvariantCulture
                        ),

                    ClockOutTime =
                        string.IsNullOrWhiteSpace(dto.clockOutTime)
                        ? null
                        : TimeOnly.ParseExact(
                            dto.clockOutTime,
                            "HH:mm",
                            CultureInfo.InvariantCulture
                        ),

                    ActionTime =
                        TimeOnly.ParseExact(
                            dto.ActionTime,
                            "HH:mm",
                            CultureInfo.InvariantCulture
                        ),

                    AttendanceDate =
                        DateOnly.FromDateTime(dto.AttendanceDate),

                    ActionType = dto.ActionType,

                    Status = "Present",

                    CreatedBy = userId,

                    CreatedAt = DateTime.Now
                };

                // ✅ SAVE RECORD
                await _unitOfWork
                    .Repository<ClockInOut>()
                    .AddAsync(entity);

                await _unitOfWork.CompleteAsync();
                // ================= NOTIFICATION =================

                var employeeUser = await _context.Users
                    .FirstOrDefaultAsync(x => x.UserId == userId);

                if (employeeUser != null)
                {
                    var notifyUsers = new List<int>();

                    // Manager
                    if (employeeUser.ReportingTo.HasValue)
                    {
                        var manager = await _context.Users
                            .FirstOrDefaultAsync(x => x.UserId == employeeUser.ReportingTo.Value);

                        if (manager != null)
                        {
                            notifyUsers.Add(manager.UserId);
                        }
                    }

                    // Reporting HR
                    if (employeeUser.ReportingHr.HasValue)
                    {
                        notifyUsers.Add(employeeUser.ReportingHr.Value);
                    }

                    notifyUsers = notifyUsers.Distinct().ToList();

                    if (notifyUsers.Any())
                    {
                        string title = dto.ActionType == "ClockIn"
                            ? "Employee Clock In"
                            : "Employee Clock Out";

                        string message = dto.ActionType == "ClockIn"
                            ? $"{employeeUser.FullName} has clocked in at {dto.ActionTime}."
                            : $"{employeeUser.FullName} has clocked out at {dto.ActionTime}.";

                        await _notificationService.CreateNotificationAsync(
                            notifyUsers,
                            title,
                            message,
                            "Attendance",
                            entity.ClockInOutId // Replace with your actual PK column if different
                        );
                    }
                }

                var attendanceDate = DateOnly.FromDateTime(dto.AttendanceDate);

                var dayLogs = await _context.ClockInOuts
                    .Where(x =>
                        x.EmployeeCode == dto.EmployeeCode &&
                        x.AttendanceDate == attendanceDate)
                    .OrderBy(x => x.ActionTime)
                    .ToListAsync();

                var lastRecord = dayLogs.LastOrDefault();

                if (lastRecord != null)
                {
                    // Last action ClockIn ante employee clockout cheyyaledu
                    if (lastRecord.ActionType == "ClockIn")
                    {
                        lastRecord.Status = "Pending Regulation";

                        lastRecord.RegulationRequested = false;
                        lastRecord.RegulationStatus = null;

                        await _context.SaveChangesAsync();
                    }
                }

                var logs = await _context.ClockInOuts
                .Where(x =>
                    x.EmployeeCode == dto.EmployeeCode &&
                    x.AttendanceDate == DateOnly.FromDateTime(dto.AttendanceDate))
                .OrderBy(x => x.ActionTime)
                .ToListAsync();
                var firstClockIn = logs
                .Where(x => x.ActionType == "ClockIn")
                .FirstOrDefault();

                var lastClockOut = logs
                    .Where(x => x.ActionType == "ClockOut")
                    .LastOrDefault();

                TimeSpan totalWorked = TimeSpan.Zero;

                TimeOnly? lastIn = null;

                foreach (var log in logs)
                {
                    if (log.ActionType == "ClockIn")
                    {
                        lastIn = log.ActionTime;
                    }
                    else if (log.ActionType == "ClockOut" && lastIn != null)
                    {
                        totalWorked += (log.ActionTime - lastIn.Value);
                        lastIn = null;
                    }
                }

                // ✅ FIX HERE
                string worked = totalWorked.ToString(@"hh\:mm\:ss");
                string firstIn = firstClockIn != null
                    ? firstClockIn.ActionTime.ToString("HH:mm")
                    : "-";

                string lastOut = lastClockOut != null
                    ? lastClockOut.ActionTime.ToString("HH:mm")
                    : "-";

                // =====================================================
                // ✅ EARLY CLOCK OUT EMAIL
                // =====================================================

                Console.WriteLine("ActionType: " + dto.ActionType);
                Console.WriteLine("TotalWorkedHours: " + dto.TotalWorkedHours);
                Console.WriteLine("UserId: " + userId);

                if (dto.ActionType == "ClockOut" && totalWorked.TotalHours > 0)
                {
                    TimeSpan workedHours = totalWorked;

                    TimeSpan requiredHours = TimeSpan.FromHours(8);

                    if (workedHours < requiredHours)
                    {
                        Console.WriteLine("Worked Less Than 8 Hours");

                        var employee = await _context.Users
                            .Where(x => x.UserId == userId)
                            .Select(x => new
                            {
                                x.FullName,
                                x.Email
                            })
                            .FirstOrDefaultAsync();

                        if (employee != null && !string.IsNullOrWhiteSpace(employee.Email))
                        {
                            TimeSpan remaining = requiredHours - workedHours;

                            if (remaining < TimeSpan.Zero)
                                remaining = TimeSpan.Zero;

                            // ✅ EMAIL BODY
                            string body = $@"

<div style='font-family:Segoe UI,Arial,sans-serif;
background-color:#f4f6f9;padding:20px;'>

<div style='max-width:600px;
margin:auto;
background:#ffffff;
border-radius:10px;
overflow:hidden;
box-shadow:0 4px 12px rgba(0,0,0,0.15);'>

<div style='background:#dc3545;
color:#ffffff;
padding:18px;
text-align:center;
font-size:22px;
font-weight:bold;'>

Early Clock Out Alert

</div>

<div style='padding:25px;
color:#333;
font-size:15px;'>

<p>
Dear <b>{employee.FullName}</b>,
</p>

<p>
You have clocked out before completing
<b>8 working hours</b>.
</p>

<table style='width:100%;
border-collapse:collapse;
margin-top:15px;
font-size:14px;'>

<tr>
<td style='padding:10px;
border:1px solid #ddd;
background:#f8f9fa;
font-weight:bold;'>
Total Worked Hours
</td>

<td style='padding:10px;
border:1px solid #ddd;'>
{worked}
</td>
</tr>

<tr>
<td style='padding:10px;
border:1px solid #ddd;
background:#f8f9fa;
font-weight:bold;'>
Required Hours
</td>

<td style='padding:10px;
border:1px solid #ddd;'>
08:00:00
</td>
</tr>

<tr>
<td style='padding:10px;
border:1px solid #ddd;
background:#f8f9fa;
font-weight:bold;'>
Remaining Hours
</td>

<td style='padding:10px;
border:1px solid #ddd;
color:#dc3545;
font-weight:bold;'>
{remaining.Hours.ToString("00")}:{remaining.Minutes.ToString("00")}:{remaining.Seconds.ToString("00")} Hours
</td>
</tr>

<tr>
<td style='padding:10px;
border:1px solid #ddd;
background:#f8f9fa;
font-weight:bold;'>
Attendance Date
</td>

<td style='padding:10px;
border:1px solid #ddd;'>
{DateTime.Now:dd-MM-yyyy}
</td>
</tr>

</table>

</div>

<div style='background:#f1f1f1;
padding:12px;
text-align:center;
font-size:12px;
color:#777;'>

© {DateTime.Now.Year}
Cortracker360 HRMS System

</div>

</div>

</div>";

                            try
                            {
                                Console.WriteLine(
                                    "Sending Email..."
                                );

                                await _emailService.SendEmailAsync(
                                    employee.Email,
                                    "Early Clock Out Alert",
                                    body,
                                    null
                                );

                                Console.WriteLine(
                                    "Email Sent Successfully"
                                );
                            }
                            catch (Exception mailEx)
                            {
                                Console.WriteLine(
                                    "Mail Error: "
                                    + mailEx.Message
                                );
                            }
                        }
                    }
                }

                return MapToDto(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(
                    "Error adding ClockInOut record: "
                    + ex.Message
                );
            }
        }  
        public async Task<bool> DeleteAsync(int id, int userId)
        {
            var entity = await _unitOfWork.Repository<ClockInOut>().GetByIdAsync(id);
            if (entity == null) return false;

            _unitOfWork.Repository<ClockInOut>().Remove(entity);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        private static ClockInOutDto MapToDto(ClockInOut entity)
        {
            return new ClockInOutDto
            {
                ClockInOutId = entity.ClockInOutId,
                RegionId = entity.RegionId,
                CompanyId = entity.CompanyId,
                EmployeeCode = entity.EmployeeCode,
                EmployeeName = entity.EmployeeName,
                Department = entity.Department,
                AttendanceDate = entity.AttendanceDate.ToDateTime(TimeOnly.MinValue),
                ActionType = entity.ActionType!,
                ActionTime = entity.ActionTime,
                //    ? entity.ActionTime.ToString(@"hh\:mm")
                //    : null,
                Status = entity.Status
            };
        }

        public async Task<IEnumerable<object>> GetWeeklyByEmployeeAsync(string employeeCode)
        {
            var data = await _unitOfWork.Repository<ClockInOut>().GetAllAsync();

            // ✅ Filter by employeeCode
            var employeeData = data
                .Where(x => x.EmployeeCode == employeeCode)
                .OrderBy(x => x.AttendanceDate)
                .ThenBy(x => x.ActionTime)
                .ToList();

            // ✅ Group by date (FIXED)
            var result = employeeData
                .GroupBy(x => x.AttendanceDate)
                .Select(g =>
                {
                    double totalMinutes = 0;
                    TimeOnly? lastIn = null;

                    foreach (var record in g)
                    {
                        if (record.ActionType == "ClockIn")
                        {
                            lastIn = record.ActionTime;
                        }
                        else if (record.ActionType == "ClockOut" && lastIn != null)
                        {
                            var diff = (record.ActionTime - lastIn.Value).TotalMinutes;

                            if (diff > 0)
                                totalMinutes += diff;

                            lastIn = null;
                        }
                    }

                    return new
                    {
                        AttendanceDate = g.Key.ToString("yyyy-MM-dd"),
                        TotalHours = Math.Round(totalMinutes / 60, 2)
                    };
                })
                .ToList();

            return result;
        }
    }
}
