using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using Microsoft.EntityFrameworkCore;

namespace BusinessLayer.Implementations
{
    public class WorkFromHomeRequestService:IWorkFromHomeRequestService
    {
        private readonly HRMSContext _context;
        private readonly IEmailService _emailService;
        private readonly INotificationService _notificationService;

        public WorkFromHomeRequestService(HRMSContext context, IEmailService emailService, INotificationService notificationService)
        {
            _context = context;
            _emailService = emailService;
            _notificationService = notificationService;
        }

        // 🔹 CREATE WFH / REMOTE REQUEST
        public async Task<WfhremoteRequest> CreateWorkFromHomeRequest(
             WfhRequestCreateDto dto)
        {
            try
            {
                var entity = new WfhremoteRequest
                {
                    EmployeeId = dto.EmployeeID,
                    EmployeeName = dto.EmployeeName,
                    FromDate = dto.FromDate,
                    ToDate = dto.ToDate,
                    RequestType = dto.RequestType,
                    Reason = dto.Reason,
                    DocumentPath = dto.DocumentPath,
                    Status = "Pending",
                    ManagerId = dto.ManagerID,
                    CompanyId = dto.CompanyID,
                    RegionId = dto.RegionID,
                    CreatedOn = DateTime.UtcNow,
                    CreatedBy = dto.UserId,
                    HrEmail = dto.HrEmail // ✅ ADD THIS
                };

                _context.WfhremoteRequests.Add(entity);

                var isDuplicate = await _context.WfhremoteRequests
    .AnyAsync(x =>
        x.EmployeeId == dto.EmployeeID &&
        x.CompanyId == dto.CompanyID &&
        x.Status != "Rejected" && // optional rule
        (
            // overlapping condition
            dto.FromDate <= x.ToDate &&
            dto.ToDate >= x.FromDate
        )
    );

                if (isDuplicate)
                {
                    throw new Exception("WFH request already exists for selected dates");
                }
                // Get Employee Details
                var employeeUser = await _context.Users
                    .FirstOrDefaultAsync(x => x.UserId == dto.UserId);

                string? reportingHrEmail = null;

                if (employeeUser?.ReportingHr != null)
                {
                    var reportingHrUser = await _context.Users
                        .FirstOrDefaultAsync(x => x.UserId == employeeUser.ReportingHr);

                    reportingHrEmail = reportingHrUser?.Email;
                }
                // GET MANAGER EMAIL
                var manager = await _context.Users
                    .FirstOrDefaultAsync(u => u.UserId == dto.ManagerID);
                // ================= NOTIFICATION SECTION =================

                var notificationUsers = new List<int>();

                // Manager Notification
                if (manager != null)
                {
                    notificationUsers.Add(manager.UserId);
                }


                // Reporting HR Notification
                if (employeeUser?.ReportingHr != null)
                {
                    notificationUsers.Add(employeeUser.ReportingHr.Value);
                }


                notificationUsers = notificationUsers
                    .Distinct()
                    .ToList();


                if (notificationUsers.Any())
                {
                    await _notificationService.CreateNotificationAsync(
                        notificationUsers,
                        "WFH Request",
                        $"{dto.EmployeeName} has submitted a Work From Home request.",
                        "Work From Home",
                        entity.WfhrequestId
                    );
                }

                if (manager != null && !string.IsNullOrEmpty(manager.Email))
                {
                    var subject = $"WFH Request Submitted - {dto.EmployeeName}";

                    var body = $@"
        <p>Dear Manager,</p>
        <p>New WFH request submitted.</p>

        <table>
            <tr><td><b>Employee</b></td><td>: {dto.EmployeeName}</td></tr>
            <tr><td><b>From</b></td><td>: {dto.FromDate}</td></tr>
            <tr><td><b>To</b></td><td>: {dto.ToDate}</td></tr>
            <tr><td><b>Type</b></td><td>: {dto.RequestType}</td></tr>
            <tr><td><b>Reason</b></td><td>: {dto.Reason}</td></tr>
        </table>
    ";

                    // ✅ CC LIST
                    var ccList = new List<string>();

                    if (!string.IsNullOrWhiteSpace(reportingHrEmail))
                    {
                        ccList.Add(reportingHrEmail);
                    }
                    if (!string.IsNullOrWhiteSpace(dto.HrEmail))
                    {
                        ccList.Add(dto.HrEmail);
                    }

                    await _emailService.SendEmailAsync(
                        manager.Email,
                        subject,
                        body,
                        ccList
                    );
                }
                await _context.SaveChangesAsync();
                return entity;



            }
            catch (Exception)
            {
                throw;
            }
        }

        // 🔹 EMPLOYEE – MY REQUESTS
        public async Task<IEnumerable<WfhremoteRequest>> GetMyWorkFromHomeRequests(
            int employeeId, int companyId, int? regionId)
        {
            return await _context.WfhremoteRequests
                .Where(x =>
                    x.EmployeeId == employeeId &&
                    x.CompanyId == companyId &&
                    (regionId == null || x.RegionId == regionId))
                .OrderByDescending(x => x.CreatedOn)
                .ToListAsync();
        }

        // 🔹 MANAGER – PENDING APPROVAL LIST
        public async Task<IEnumerable<WfhremoteRequest>> GetPendingWorkFromHomeRequests(
            int companyId, int? regionId, int managerId)
        {
            return await _context.WfhremoteRequests
                .Where(x =>
                   
                    x.ManagerId == managerId &&
                     x.CompanyId == companyId &&
            (regionId == null || x.RegionId == regionId))
        .OrderByDescending(x => x.CreatedOn)
        .ToListAsync();
        }
        

        // 🔹 SINGLE APPROVE / REJECT
        public async Task<bool> UpdateWorkFromHomeRequest(
            UpdateWorkFromHomeRequestDto dto)
        {
            var entity = await _context.WfhremoteRequests
                .FirstOrDefaultAsync(x =>
                    x.WfhrequestId == dto.WFHRequestID &&
                    x.CompanyId == dto.CompanyID &&
                    (dto.RegionID == null || x.RegionId == dto.RegionID) &&
                    x.Status == "Pending");

            if (entity == null)
                return false;

            entity.Status = dto.Status;
            entity.ManagerRemarks = dto.ManagerRemarks;
            entity.ManagerId = dto.ManagerID;
            entity.ApprovedOn = dto.Status == "Approved"
                ? DateTime.UtcNow
                : null;
            entity.UpdatedOn = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var employee = await _context.Users
        .FirstOrDefaultAsync(u => u.UserId == entity.EmployeeId);

            var notificationUsers = new List<int>();

            // Employee Notification
            notificationUsers.Add(entity.EmployeeId);


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
                    "WFH Request",
                    $"{entity.EmployeeName}'s Work From Home request has been {entity.Status} by Manager.",
                    "Work From Home",
                    entity.WfhrequestId
                );
            }

            if (employee != null && !string.IsNullOrEmpty(employee.Email))
            {
                var subject = $"WFH Request {entity.Status}";

                var body = $@"
        <p>Dear {entity.EmployeeName},</p>
        <p>Your Work From Home request has been <b>{entity.Status}</b>.</p>

        <table>
            <tr><td><b>From</b></td><td>: {entity.FromDate}</td></tr>
            <tr><td><b>To</b></td><td>: {entity.ToDate}</td></tr>
            <tr><td><b>Status</b></td><td>: {entity.Status}</td></tr>
            <tr><td><b>Remarks</b></td><td>: {entity.ManagerRemarks}</td></tr>
        </table>
        ";

                // ✅ CC (IMPORTANT FIX)
                var ccList = new List<string>();

                if (!string.IsNullOrWhiteSpace(entity.HrEmail))
                {
                    ccList.Add(entity.HrEmail); // 🔥 SAME CC EMAIL RETAINED
                }

                await _emailService.SendEmailAsync(
                    employee.Email,
                    subject,
                    body,
                    ccList
                );
            }
            return true;
        }

        // 🔥 BULK APPROVE / REJECT
        public async Task<int> BulkApproveRejectWorkFromHome(
            BulkApproveRejectWorkFromHomeDto dto)
        {
            var records = await _context.WfhremoteRequests
                .Where(x =>
                    dto.WFHRequestIDs.Contains(x.WfhrequestId) &&
                   
                    x.Status == "Pending")
                .ToListAsync();

            if (!records.Any())
                return 0;

            foreach (var item in records)
            {
                item.Status = dto.Status;
                item.ManagerRemarks = dto.ManagerRemarks;
                item.ManagerId = dto.ManagerID;
                item.ApprovedOn = dto.Status == "Approved"
                    ? DateTime.UtcNow
                    : null;
                item.UpdatedOn = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            foreach (var item in records)
            {
                var employee = await _context.Users
                    .FirstOrDefaultAsync(u => u.UserId == item.EmployeeId);

                if (employee != null)
                {
                    var notificationUsers = new List<int>();

                    // Employee Notification
                    notificationUsers.Add(item.EmployeeId);


                    // Reporting HR Notification
                    if (employee.ReportingHr.HasValue)
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
                            "WFH Request",
                            $"{item.EmployeeName}'s Work From Home request has been {item.Status} by Manager.",
                            "Work From Home",
                            item.WfhrequestId
                        );
                    }
                }




                if (employee != null && !string.IsNullOrEmpty(employee.Email))
                {
                    string? reportingHrEmail = null;

                    if (employee.ReportingHr.HasValue)
                    {
                        var reportingHrUser = await _context.Users
                            .FirstOrDefaultAsync(x => x.UserId == employee.ReportingHr.Value);

                        reportingHrEmail = reportingHrUser?.Email;
                    }


                    var subject = $"WFH Request {item.Status}";

                    var body = $@"
            <p>Dear {item.EmployeeName},</p>
            <p>Your Work From Home request has been <b>{item.Status}</b>.</p>

            <table>
                <tr><td><b>From</b></td><td>: {item.FromDate}</td></tr>
                <tr><td><b>To</b></td><td>: {item.ToDate}</td></tr>
                <tr><td><b>Status</b></td><td>: {item.Status}</td></tr>
                <tr><td><b>Remarks</b></td><td>: {item.ManagerRemarks}</td></tr>
            </table>
            ";

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

                    // Remove duplicates
                    ccList = ccList.Distinct().ToList();

                    await _emailService.SendEmailAsync(
                        employee.Email,
                        subject,
                        body,
                        ccList
                    );
                }
            }

            return records.Count;
        }
    }
}
