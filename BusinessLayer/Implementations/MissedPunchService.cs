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

        public MissedPunchService(HRMSContext context, IEmailService emailService) // ✅ ADD
        {
            _context = context;
            _emailService = emailService; // ✅ ADD
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
                var employee = await _context.Users
                    .Where(x => x.UserId == dto.UserId)
                    .Select(x => new { x.FullName, x.Email })
                    .FirstOrDefaultAsync();

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

                    await _emailService.SendEmailAsync(
                        manager.Email,
                        "New Missed Punch Request",
                        body,
                        string.IsNullOrEmpty(dto.HrEmail)
                            ? null
                            : new List<string> { dto.HrEmail } // ✅ convert string → list
               
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
        public async Task<IEnumerable<MissedPunchRequest>> GetApprovalMissedPunchRequest(
            int companyId, int? regionId, int managerId)
        {
            var result= await _context.MissedPunchRequests
                .Where(x =>
                   
                    x.Status == "Pending" && x.ManagerId==managerId)
                .OrderBy(x => x.MissedDate)
                .ToListAsync();

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

            await _context.SaveChangesAsync();

            // ===============================
            // ✅ EMAIL LOGIC START
            // ===============================

            // 🔹 GET EMPLOYEE DETAILS
            var employee = await _context.Users
                .Where(x => x.UserId == entity.UserId)
                .Select(x => new { x.Email, x.FullName })
                .FirstOrDefaultAsync();

            // 🔹 GET MANAGER DETAILS (optional if needed)
            var manager = await _context.Users
                .Where(x => x.UserId == dto.ManagerID)
                .Select(x => new { x.FullName })
                .FirstOrDefaultAsync();

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

                await _emailService.SendEmailAsync(
                    employee.Email,
                    $"Missed Punch Request {dto.Status}",
                    body,
                    string.IsNullOrEmpty(entity.HrEmail)
                        ? null
                        : new List<string> { entity.HrEmail }
            
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

                // ✅ GET EMPLOYEE DETAILS
                var employee = await _context.Users
                    .Where(x => x.UserId == item.UserId)
                    .Select(x => new { x.Email, x.FullName })
                    .FirstOrDefaultAsync();

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

                    await _emailService.SendEmailAsync(
                        employee.Email,
                        $"Missed Punch Request {dto.Status}",
                        body,
                        string.IsNullOrEmpty(item.HrEmail)
                            ? null
                            : new List<string> { item.HrEmail }
                
                    );
                }
            }
            await _context.SaveChangesAsync();
            return records.Count;
        }
    }
}
