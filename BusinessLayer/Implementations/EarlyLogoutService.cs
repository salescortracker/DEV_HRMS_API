using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Implementations
{
    public class EarlyLogoutService : IEarlyLogoutService
    {
        private readonly HRMSContext _context;
        private readonly IEmailService _emailService;

        public EarlyLogoutService(HRMSContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }
      

        public async Task<int> CreateEarlyLogoutRequest(CreateEarlyLogoutRequestDto dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.UserId == dto.UserId);

            if (user == null)
            {
                throw new Exception("User not found");
            }

            var duplicateRequest = await _context.EarlyLogoutRequests
            .FirstOrDefaultAsync(x =>
                x.UserId == dto.UserId &&
                x.CompanyId == dto.CompanyID &&
                x.RequestDate == dto.RequestDate);

            if (duplicateRequest != null)
            {
                throw new Exception("Early Logout Request already exists for this date.");
            }


            var entity = new EarlyLogoutRequest
            {
                EmployeeId = dto.UserId,
                UserId = dto.UserId,
                RequestDate = dto.RequestDate,
                RequestedLogoutTime = dto.RequestedLogoutTime,
                ManagerId = user.ReportingTo,

                Reason = dto.Reason,
                Status = "Pending",
                CompanyId = dto.CompanyID,
                RegionId = dto.RegionID,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = dto.UserId,
                HrEmail = dto.HrEmail
            };

            _context.EarlyLogoutRequests.Add(entity);
            await _context.SaveChangesAsync();

            try
            {
                var saved = await _context.EarlyLogoutRequests
                    .Include(x => x.Employee)
                    .Include(x => x.Manager)
                    .FirstOrDefaultAsync(x => x.EarlyLogoutRequestId == entity.EarlyLogoutRequestId);

                // Employee Details
                var employee = await _context.Users
                    .Where(x => x.UserId == dto.UserId)
                    .Select(x => new
                    {
                        x.FullName,
                        x.Email,
                        x.ReportingHr
                    })
                    .FirstOrDefaultAsync();

                // Reporting HR Email
                string? reportingHrEmail = null;

                if (employee?.ReportingHr != null)
                {
                    var reportingHrUser = await _context.Users
                        .FirstOrDefaultAsync(x => x.UserId == employee.ReportingHr);

                    reportingHrEmail = reportingHrUser?.Email;
                }

                if (saved?.Manager != null && !string.IsNullOrEmpty(saved.Manager.Email))
                {
                    var body = $@"
        <div style='font-family:Arial'>
            <h3>Early Logout Request Notification</h3>
            <p>Dear {saved.Manager.FullName},</p>

            <p>A new early logout request has been submitted.</p>

            <table border='1' cellpadding='6' cellspacing='0'>
                <tr>
                    <td><b>Employee</b></td>
                    <td>{saved.Employee?.FullName}</td>
                </tr>
                <tr>
                    <td><b>Date</b></td>
                    <td>{saved.RequestDate:dd-MM-yyyy}</td>
                </tr>
                <tr>
                    <td><b>Requested Logout Time</b></td>
                    <td>{saved.RequestedLogoutTime}</td>
                </tr>
                <tr>
                    <td><b>Reason</b></td>
                    <td>{saved.Reason}</td>
                </tr>
            </table>

            <br/>
            <p>Regards,<br/><b>HRMS Team</b></p>
        </div>";

                    var ccList = new List<string>();

                    // Reporting HR Email
                    if (!string.IsNullOrWhiteSpace(reportingHrEmail))
                    {
                        ccList.Add(reportingHrEmail);
                    }

                    // Optional CC Emails from UI
                    if (!string.IsNullOrWhiteSpace(dto.HrEmail))
                    {
                        ccList.AddRange(
                            dto.HrEmail
                                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                                .Select(x => x.Trim())
                                .Where(x => !string.IsNullOrWhiteSpace(x))
                        );
                    }

                    ccList = ccList.Distinct().ToList();

                    await _emailService.SendEmailAsync(
                        saved.Manager.Email,
                        "New Early Logout Request",
                        body,
                        ccList.Any() ? ccList : null
                    );
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email Error: {ex.Message}");
            }

            return entity.EarlyLogoutRequestId;
        }

        public async Task<IEnumerable<EarlyLogoutRequest>> GetEarlyLogoutRequest(int companyId, int? regionId, int userId)
        {
            return await _context.EarlyLogoutRequests
                .Where(x =>
                    x.CompanyId == companyId &&
                    x.UserId == userId &&
                    (regionId == null || x.RegionId == regionId))
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<EarlyLogoutApprovalListDto>> GetApprovalEarlyLogoutRequest(int companyId, int? regionId, int managerId)
        {
            var result = await (
                from el in _context.EarlyLogoutRequests
                join u in _context.Users on el.UserId equals u.UserId
                where el.ManagerId == managerId
                      && el.CompanyId == companyId
                      && (regionId == null || el.RegionId == regionId)
                orderby el.RequestDate
                select new EarlyLogoutApprovalListDto
                {
                    EarlyLogoutRequestId = el.EarlyLogoutRequestId,
                    UserId = el.UserId,
                    EmployeeName = u.FullName,
                    RequestDate = el.RequestDate,
                    RequestedLogoutTime = el.RequestedLogoutTime,
                    Reason = el.Reason,
                    HrEmail = el.HrEmail,
                    Status = el.Status,
                    ManagerRemarks = el.ManagerRemarks
                }
            ).ToListAsync();

            return result;
        }




        public async Task<bool> UpdateEarlyLogout(UpdateEarlyLogoutDto dto)
        {
            var entity = await _context.EarlyLogoutRequests
                .FirstOrDefaultAsync(x =>
                    x.EarlyLogoutRequestId == dto.EarlyLogoutRequestID &&
                    x.CompanyId == dto.CompanyID &&
                    (dto.RegionID == null || x.RegionId == dto.RegionID) &&
                    x.Status == "Pending");

            if (entity == null)
                return false;



            var duplicate = await _context.EarlyLogoutRequests
            .AnyAsync(x =>
                x.UserId == entity.UserId &&
                x.RequestDate == dto.RequestDate &&
                x.EarlyLogoutRequestId != dto.EarlyLogoutRequestID);

            if (duplicate)
            {
                throw new Exception("Early Logout Request already exists for this date.");
            }



            entity.RequestDate = dto.RequestDate;
            entity.RequestedLogoutTime = dto.RequestedLogoutTime;
            entity.Reason = dto.Reason;
            entity.HrEmail = dto.HrEmail;
            entity.ModifiedAt = DateTime.UtcNow;
            entity.ModifiedBy = entity.UserId;

            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<int> BulkApproveRejectEarlyLogout(BulkApproveRejectEarlyLogoutDto dto)
        {
            var records = await _context.EarlyLogoutRequests
                .Include(x => x.Employee)
                .Where(x =>
                    dto.EarlyLogoutRequestIds.Contains(x.EarlyLogoutRequestId) &&
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

                try
                {
                    // Get Employee Details
                    var employee = await _context.Users
                        .FirstOrDefaultAsync(x => x.UserId == item.UserId);

                    if (employee != null && !string.IsNullOrWhiteSpace(employee.Email))
                    {
                        var body = $@"
            <div style='font-family:Arial'>
                <h3>Early Logout Request Update</h3>

                <p>Dear {employee.FullName},</p>

                <p>Your early logout request has been
                <b>{dto.Status}</b>.</p>

                <table border='1' cellpadding='6' cellspacing='0'>
                    <tr>
                        <td><b>Date</b></td>
                        <td>{item.RequestDate:dd-MM-yyyy}</td>
                    </tr>
                    <tr>
                        <td><b>Requested Logout Time</b></td>
                        <td>{item.RequestedLogoutTime}</td>
                    </tr>
                    <tr>
                        <td><b>Reason</b></td>
                        <td>{item.Reason}</td>
                    </tr>
                    <tr>
                        <td><b>Manager Remarks</b></td>
                        <td>{dto.ManagerRemarks}</td>
                    </tr>
                    <tr>
                        <td><b>Status</b></td>
                        <td>{dto.Status}</td>
                    </tr>
                </table>

                <br/>
                <p>Regards,<br/><b>HRMS Team</b></p>
            </div>";

                        var ccList = new List<string>();

                        // Reporting HR Email
                        if (employee.ReportingHr != null)
                        {
                            var reportingHr = await _context.Users
                                .FirstOrDefaultAsync(x => x.UserId == employee.ReportingHr);

                            if (!string.IsNullOrWhiteSpace(reportingHr?.Email))
                            {
                                ccList.Add(reportingHr.Email);
                            }
                        }

                        // Additional HR Emails entered in UI
                        if (!string.IsNullOrWhiteSpace(item.HrEmail))
                        {
                            ccList.AddRange(
                                item.HrEmail
                                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                                    .Select(x => x.Trim())
                                    .Where(x => !string.IsNullOrWhiteSpace(x))
                            );
                        }

                        ccList = ccList.Distinct().ToList();

                        Console.WriteLine($"Employee Email : {employee.Email}");
                        Console.WriteLine($"CC Emails      : {string.Join(",", ccList)}");

                        await _emailService.SendEmailAsync(
                            employee.Email,                     // TO Employee
                            $"Early Logout Request {dto.Status}",
                            body,
                            ccList.Any() ? ccList : null        // CC HR
                        );
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Email Error: {ex.Message}");
                }
            }

            await _context.SaveChangesAsync();
            return records.Count;
        }



        private async Task UpdateClockOutIfApproved(EarlyLogoutRequest entity)
        {
            var user = await _context.Users
                .Where(x =>
                    x.UserId == entity.UserId &&
                    x.CompanyId == entity.CompanyId &&
                    x.RegionId == (entity.RegionId ?? 0))
                .Select(x => new { x.EmployeeCode, x.FullName })
                .FirstOrDefaultAsync();

            if (user == null) return;

            var existingOut = await _context.ClockInOuts
                .Where(x =>
                    x.EmployeeCode == user.EmployeeCode &&
                    x.CompanyId == entity.CompanyId &&
                    x.RegionId == (entity.RegionId ?? 0) &&
                    x.AttendanceDate == entity.RequestDate &&
                    x.ActionType == "ClockOut")
                .OrderByDescending(x => x.ActionTime)
                .FirstOrDefaultAsync();

            if (existingOut != null)
            {
                existingOut.ActionTime = entity.RequestedLogoutTime;
                existingOut.ClockOutTime = entity.RequestedLogoutTime;
                existingOut.ModifiedAt = DateTime.UtcNow;
                existingOut.ModifiedBy = entity.ModifiedBy;
            }
            else
            {
                _context.ClockInOuts.Add(new ClockInOut
                {
                    EmployeeCode = user.EmployeeCode,
                    EmployeeName = user.FullName,
                    CompanyId = entity.CompanyId,
                    RegionId = entity.RegionId ?? 0,
                    AttendanceDate = entity.RequestDate,
                    ActionType = "ClockOut",
                    ActionTime = entity.RequestedLogoutTime,
                    ClockOutTime = entity.RequestedLogoutTime,
                    Status = "Approved Early Logout",
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = entity.ModifiedBy
                });
            }
        }

    }
}
