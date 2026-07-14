using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using Microsoft.EntityFrameworkCore;

namespace BusinessLayer.Implementations
{
    public class MissedPunchService: IMissedPunchService
    {
        private readonly HRMSContext _context;
        private readonly IEmailService _emailService; // ✅ ADD
        private readonly INotificationService _notificationService;

        public MissedPunchService(HRMSContext context, IEmailService emailService, INotificationService notificationService) // ✅ ADD
        {
            _context = context;
            _emailService = emailService; // ✅ ADD
            _notificationService = notificationService;
        }

        // 🔹 CREATE
        public async Task<MissedPunchRequest> CreateMissedPunchRequest(
            CreateMissedPunchRequestDto dto)
        {
            try
            {
                var entity = new MissedPunchRequest
                {
                    EmployeeId = dto.EmployeeID,
                    MissedDate = dto.MissedDate,
                    MissedType = dto.MissedType,
                    ManagerId=dto.reportingTo,
                    CorrectClockIn = dto.CorrectClockIn,
                    CorrectClockOut = dto.CorrectClockOut,
                    Reason = dto.Reason,
                    Status = "Pending",
                    CompanyId = dto.CompanyID,
                    RegionId = dto.RegionID,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = dto.UserId,
                    UserId = dto.UserId,
                    HrEmail = dto.HrEmail,
                };

                _context.MissedPunchRequests.Add(entity);
                await _context.SaveChangesAsync();
                // ✅ GET MANAGER DETAILS
                var manager = await _context.Users
                    .Where(x => x.UserId == dto.reportingTo)
                    .Select(x => new { x.Email, x.FullName })
                    .FirstOrDefaultAsync();

                // ✅ GET EMPLOYEE DETAILS
                //var employee = await _context.Users
                //    .Where(x => x.UserId == dto.UserId)
                //    .Select(x => new { x.FullName, x.Email })
                //    .FirstOrDefaultAsync();
                var employee = await _context.Users
    .Where(x => x.UserId == dto.UserId)
    .Select(x => new
    {
        x.FullName,
        x.Email,
        x.ReportingHr
    })
    .FirstOrDefaultAsync();

                string? reportingHrEmail = null;

                if (employee?.ReportingHr != null)
                {
                    var reportingHrUser = await _context.Users
                        .FirstOrDefaultAsync(x => x.UserId == employee.ReportingHr);

                    reportingHrEmail = reportingHrUser?.Email;
                }
                // ================= NOTIFICATION SECTION =================

                var notificationUsers = new List<int>();

                if (dto.reportingTo.HasValue)
                {
                    notificationUsers.Add(dto.reportingTo.Value);
                }

                if (employee?.ReportingHr != null)
                {
                    notificationUsers.Add(employee.ReportingHr.Value);
                }

                notificationUsers = notificationUsers.Distinct().ToList();

                if (notificationUsers.Any())
                {
                    await _notificationService.CreateNotificationAsync(
                        notificationUsers,
                        "Missed Punch Request",
                        $"{employee.FullName} has submitted a missed punch request for {dto.MissedDate:dd-MMM-yyyy}.",
                        "Attendance",
                        entity.MissedPunchRequestId   // Replace with your actual PK if needed
                    );
                }
                // ✅ SEND EMAIL TO MANAGER
                if (manager != null && !string.IsNullOrEmpty(manager.Email))
                {
                    var body = $@"
    <div style='font-family:Arial'>
        <h3>Missed Punch Request Notification</h3>

        <p>Dear {manager.FullName},</p>

        <p>A new missed punch request has been submitted.</p>

        <table border='1' cellpadding='6' cellspacing='0'>
            <tr><td><b>Employee</b></td><td>{employee?.FullName}</td></tr>
            <tr><td><b>Date</b></td><td>{dto.MissedDate:dd-MM-yyyy}</td></tr>
            <tr><td><b>Type</b></td><td>{dto.MissedType}</td></tr>
            <tr><td><b>Reason</b></td><td>{dto.Reason}</td></tr>
        </table>

        <p>Please review and take action.</p>

        <br/>
        <p>Regards,<br/><b>HRMS Team</b></p>
    </div>
    ";

                    //await _emailService.SendEmailAsync(
                    //    manager.Email,
                    //    "New Missed Punch Request",
                    //    body,
                    //    string.IsNullOrEmpty(dto.HrEmail)
                    //        ? null
                    //        : new List<string> { dto.HrEmail } // ✅ convert string → list

                    //);

                    var ccList = new List<string>();

                    // Reporting HR
                    if (!string.IsNullOrWhiteSpace(reportingHrEmail))
                    {
                        ccList.Add(reportingHrEmail);
                    }

                    // UI CC Emails
                    if (!string.IsNullOrWhiteSpace(dto.HrEmail))
                    {
                        ccList.AddRange(
                            dto.HrEmail
                                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                                .Select(x => x.Trim())
                                .Where(x => !string.IsNullOrEmpty(x))
                        );
                    }

                    ccList = ccList.Distinct().ToList();

                    await _emailService.SendEmailAsync(
                        manager.Email,
                        "New Missed Punch Request",
                        body,
                        ccList
                    );
                }
                return entity;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        // 🔹 EMPLOYEE LIST
        public async Task<IEnumerable<MissedPunchRequest>> GetMissedPunchRequest(
            int companyId, int? regionId, int userId)
        {
            return await _context.MissedPunchRequests
                .Where(x =>
                    x.CompanyId == companyId &&
                    x.UserId == userId && // ✅ IMPORTANT FILTER
                    (regionId == null || x.RegionId == regionId))
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        // 🔹 MANAGER APPROVAL LIST
        public async Task<IEnumerable<MissedPunchApprovalListDto>> GetApprovalMissedPunchRequest(
        int companyId, int? regionId, int managerId)
        {
            var result = await (
                from mp in _context.MissedPunchRequests
                join u in _context.Users
                    on mp.UserId equals u.UserId
                where mp.ManagerId == managerId
                      && mp.CompanyId == companyId
                      && (regionId == null || mp.RegionId == regionId)
                orderby mp.MissedDate
                select new MissedPunchApprovalListDto
                {
                    MissedPunchRequestId = mp.MissedPunchRequestId,
                    UserId = mp.UserId,
                    EmployeeName = u.FullName,
                    MissedDate = mp.MissedDate,
                    MissedType = mp.MissedType,
                    CorrectClockIn = mp.CorrectClockIn,
                    CorrectClockOut = mp.CorrectClockOut,
                    Reason = mp.Reason,
                    HrEmail = mp.HrEmail,
                    Status = mp.Status,                // ✅ ADD THIS
                    ManagerRemarks = mp.ManagerRemarks // ✅ ADD THIS
                }
            ).ToListAsync();

            return result;
        }

        // 🔹 SINGLE APPROVE / REJECT
        public async Task<bool> UpdateMissedPunch(UpdateMissedPunchDto dto)
        {
            var entity = await _context.MissedPunchRequests
                .FirstOrDefaultAsync(x =>
                    x.MissedPunchRequestId == dto.MissedPunchRequestID &&
                    x.CompanyId == dto.CompanyID &&
                    (dto.RegionID == null || x.RegionId == dto.RegionID) &&
                    x.Status == "Pending");

            if (entity == null)
                return false;

            // ✅ UPDATE DATA
            entity.Status = dto.Status;
            entity.ManagerRemarks = dto.ManagerRemarks;
            entity.ManagerId = dto.ManagerID;
            entity.ModifiedAt = DateTime.UtcNow;
            entity.ModifiedBy = dto.ManagerID;
            entity.HrEmail = dto.HrEmail;

            await UpdateClockInOutIfChanged(entity);
            await _context.SaveChangesAsync();




            // ===============================
            // ✅ EMAIL LOGIC START
            // ===============================

            // 🔹 GET EMPLOYEE DETAILS
            //var employee = await _context.Users
            //    .Where(x => x.UserId == entity.UserId)
            //    .Select(x => new { x.Email, x.FullName })
            //    .FirstOrDefaultAsync();
            var employee = await _context.Users
    .Where(x => x.UserId == entity.UserId)
    .Select(x => new
    {
        x.Email,
        x.FullName,
        x.ReportingHr
    })
    .FirstOrDefaultAsync();

            string? reportingHrEmail = null;

            if (employee?.ReportingHr != null)
            {
                var reportingHrUser = await _context.Users
                    .FirstOrDefaultAsync(x => x.UserId == employee.ReportingHr);

                reportingHrEmail = reportingHrUser?.Email;
            }

            // 🔹 GET MANAGER DETAILS (optional if needed)
            var manager = await _context.Users
                .Where(x => x.UserId == dto.ManagerID)
                .Select(x => new { x.FullName })
                .FirstOrDefaultAsync();
            // ================= NOTIFICATION =================

            var notificationUsers = new List<int>();

            // Employee
            notificationUsers.Add(entity.UserId);

            // Reporting HR
            if (employee?.ReportingHr != null)
            {
                notificationUsers.Add(employee.ReportingHr.Value);
            }

            notificationUsers = notificationUsers.Distinct().ToList();

            if (notificationUsers.Any())
            {
                await _notificationService.CreateNotificationAsync(
                    notificationUsers,
                    "Missed Punch Request",
                    $"{employee?.FullName}'s missed punch request has been {dto.Status} by Manager.",
                    "Attendance",
                    entity.MissedPunchRequestId
                );
            }

            if (employee != null && !string.IsNullOrEmpty(employee.Email))
            {
                var body = $@"
        <div style='font-family:Arial'>
            <h3>Missed Punch Request Update</h3>

            <p>Dear {employee.FullName},</p>

            <p>Your missed punch request has been <b>{dto.Status}</b>.</p>

            <table border='1' cellpadding='6' cellspacing='0'>
                <tr><td><b>Date</b></td><td>{entity.MissedDate:dd-MM-yyyy}</td></tr>
                <tr><td><b>Type</b></td><td>{entity.MissedType}</td></tr>
                <tr><td><b>Reason</b></td><td>{entity.Reason}</td></tr>
                <tr><td><b>Manager</b></td><td>{manager?.FullName}</td></tr>
                <tr><td><b>Manager Remarks</b></td><td>{dto.ManagerRemarks}</td></tr>
                <tr><td><b>Status</b></td><td>{dto.Status}</td></tr>
            </table>

            <br/>
            <p>Regards,<br/><b>HRMS Team</b></p>
        </div>
        ";

                //await _emailService.SendEmailAsync(
                //    employee.Email,
                //    $"Missed Punch Request {dto.Status}",
                //    body,
                //    string.IsNullOrEmpty(entity.HrEmail)
                //        ? null
                //        : new List<string> { entity.HrEmail }

                //);
                var ccList = new List<string>();

                // Reporting HR
                if (!string.IsNullOrWhiteSpace(reportingHrEmail))
                {
                    ccList.Add(reportingHrEmail);
                }

                // UI CC Emails
                if (!string.IsNullOrWhiteSpace(entity.HrEmail))
                {
                    ccList.AddRange(
                        entity.HrEmail
                            .Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(x => x.Trim())
                            .Where(x => !string.IsNullOrEmpty(x))
                    );
                }

                ccList = ccList.Distinct().ToList();

                await _emailService.SendEmailAsync(
                    employee.Email,
                    $"Missed Punch Request {dto.Status}",
                    body,
                    ccList
                );
            }

            // ===============================
            // ✅ EMAIL LOGIC END
            // ===============================

            return true;
        }

        // 🔥 BULK APPROVE / REJECT
        public async Task<int> BulkApproveRejectPunch(BulkApproveRejectPunchDto dto)
        {
            var records = await _context.MissedPunchRequests
                .Where(x =>
                    dto.MissedPunchRequestIds.Contains(x.MissedPunchRequestId) &&
                  
                    x.Status == "Pending")
                .ToListAsync();

            if (!records.Any())
                return 0;

            foreach (var item in records)
            {
                item.Status = dto.Status;
                item.ManagerRemarks = dto.ManagerRemarks;
                item.ManagerId = dto.ManagerID;
                item.ModifiedAt = DateTime.UtcNow;
                item.ModifiedBy = dto.ManagerID;

                await UpdateClockInOutIfChanged(item);

                // ✅ GET EMPLOYEE DETAILS
                //var employee = await _context.Users
                //    .Where(x => x.UserId == item.UserId)
                //    .Select(x => new { x.Email, x.FullName })
                //    .FirstOrDefaultAsync();
                var employee = await _context.Users
    .Where(x => x.UserId == item.UserId)
    .Select(x => new
    {
        x.Email,
        x.FullName,
        x.ReportingHr
    })
    .FirstOrDefaultAsync();
                // ================= NOTIFICATION SECTION =================

                var notificationUsers = new List<int>();

                // Employee Notification
                notificationUsers.Add(item.UserId);


                // Reporting HR Notification
                if (employee?.ReportingHr != null)
                {
                    notificationUsers.Add(employee.ReportingHr.Value);
                }


                notificationUsers = notificationUsers
                    .Distinct()
                    .ToList();


                if (notificationUsers.Any())
                {
                    await _notificationService.CreateNotificationAsync(
                        notificationUsers,
                        "Missed Punch Request",
                        $"{employee?.FullName}'s missed punch request has been {dto.Status} by Manager.",
                        "Attendance",
                        item.MissedPunchRequestId
                    );
                }

                string? reportingHrEmail = null;

                if (employee?.ReportingHr != null)
                {
                    var reportingHrUser = await _context.Users
                        .FirstOrDefaultAsync(x => x.UserId == employee.ReportingHr);

                    reportingHrEmail = reportingHrUser?.Email;
                }


                if (employee != null && !string.IsNullOrEmpty(employee.Email))
                {
                    var body = $@"
        <div style='font-family:Arial'>
            <h3>Missed Punch Request Update</h3>

            <p>Dear {employee.FullName},</p>

            <p>Your missed punch request has been <b>{dto.Status}</b>.</p>

            <table border='1' cellpadding='6' cellspacing='0'>
                <tr><td><b>Date</b></td><td>{item.MissedDate:dd-MM-yyyy}</td></tr>
                <tr><td><b>Type</b></td><td>{item.MissedType}</td></tr>
                <tr><td><b>Reason</b></td><td>{item.Reason}</td></tr>
                <tr><td><b>Manager Remarks</b></td><td>{dto.ManagerRemarks}</td></tr>
                <tr><td><b>Status</b></td><td>{dto.Status}</td></tr>
            </table>

            <br/>
            <p>Regards,<br/><b>HRMS Team</b></p>
        </div>
        ";

                    //await _emailService.SendEmailAsync(
                    //    employee.Email,
                    //    $"Missed Punch Request {dto.Status}",
                    //    body,
                    //    string.IsNullOrEmpty(item.HrEmail)
                    //        ? null
                    //        : new List<string> { item.HrEmail }

                    //);

                    var ccList = new List<string>();

                    // Reporting HR
                    if (!string.IsNullOrWhiteSpace(reportingHrEmail))
                    {
                        ccList.Add(reportingHrEmail);
                    }

                    // UI CC Emails
                    if (!string.IsNullOrWhiteSpace(item.HrEmail))
                    {
                        ccList.AddRange(
                            item.HrEmail
                                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                                .Select(x => x.Trim())
                                .Where(x => !string.IsNullOrEmpty(x))
                        );
                    }

                    ccList = ccList.Distinct().ToList();

                    await _emailService.SendEmailAsync(
                        employee.Email,
                        $"Missed Punch Request {dto.Status}",
                        body,
                        ccList
                    );
                }
            }
            await _context.SaveChangesAsync();
            return records.Count;

        }


        private async Task UpdateClockInOutIfChanged(MissedPunchRequest entity)
        {
            // ✅ Skip if nothing to update
            if (!entity.CorrectClockIn.HasValue && !entity.CorrectClockOut.HasValue)
                return;

            // ✅ Get employee details
            var user = await _context.Users
                .Where(x =>
                    x.UserId == entity.UserId &&
                    x.CompanyId == entity.CompanyId &&
                    x.RegionId == (entity.RegionId ?? 0))
                .Select(x => new
                {
                    x.EmployeeCode,
                    x.FullName
                })
                .FirstOrDefaultAsync();

            if (user == null) return;

            var attendanceDate = entity.MissedDate;

            // ✅ Get existing records
            var records = await _context.ClockInOuts
                .Where(x =>
                    x.EmployeeCode == user.EmployeeCode &&
                    x.CompanyId == entity.CompanyId &&
                    x.RegionId == (entity.RegionId ?? 0) &&
                    x.AttendanceDate == attendanceDate)
                .ToListAsync();

            // ================= CLOCK IN =================
            if (entity.CorrectClockIn.HasValue)
            {
                var existingIn = records
                    .Where(x => x.ActionType == "ClockIn")
                    .OrderBy(x => x.ActionTime)
                    .FirstOrDefault();

                if (existingIn != null)
                {
                    if (existingIn.ActionTime != entity.CorrectClockIn.Value)
                    {
                        existingIn.ActionTime = entity.CorrectClockIn.Value;
                        existingIn.ClockInTime = entity.CorrectClockIn.Value;
                        existingIn.ModifiedAt = DateTime.UtcNow;
                        existingIn.ModifiedBy = entity.ModifiedBy;
                    }
                }
                else
                {
                    _context.ClockInOuts.Add(new ClockInOut
                    {
                        EmployeeCode = user.EmployeeCode,
                        EmployeeName = user.FullName,
                        CompanyId = entity.CompanyId,
                        RegionId = entity.RegionId ?? 0,
                        AttendanceDate = attendanceDate,
                        ActionType = "ClockIn",
                        ActionTime = entity.CorrectClockIn.Value,
                        ClockInTime = entity.CorrectClockIn.Value,
                        Status = "Present",
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = entity.ModifiedBy
                    });
                }
            }

            // ================= CLOCK OUT =================
            if (entity.CorrectClockOut.HasValue)
            {
                var existingOut = records
                    .Where(x => x.ActionType == "ClockOut")
                    .OrderByDescending(x => x.ActionTime)
                    .FirstOrDefault();

                if (existingOut != null)
                {
                    if (existingOut.ActionTime != entity.CorrectClockOut.Value)
                    {
                        existingOut.ActionTime = entity.CorrectClockOut.Value;
                        existingOut.ClockOutTime = entity.CorrectClockOut.Value;
                        existingOut.ModifiedAt = DateTime.UtcNow;
                        existingOut.ModifiedBy = entity.ModifiedBy;
                    }
                }
                else
                {
                    _context.ClockInOuts.Add(new ClockInOut
                    {
                        EmployeeCode = user.EmployeeCode,
                        EmployeeName = user.FullName,
                        CompanyId = entity.CompanyId,
                        RegionId = entity.RegionId ?? 0,
                        AttendanceDate = attendanceDate,
                        ActionType = "ClockOut",
                        ActionTime = entity.CorrectClockOut.Value,
                        ClockOutTime = entity.CorrectClockOut.Value,
                        Status = "Completed",
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = entity.ModifiedBy
                    });
                }
            }
        }
    }
}
