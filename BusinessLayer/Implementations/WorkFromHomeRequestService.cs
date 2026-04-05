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

        public WorkFromHomeRequestService(HRMSContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
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



                // GET MANAGER EMAIL
                var manager = await _context.Users
                    .FirstOrDefaultAsync(u => u.UserId == dto.ManagerID);

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

                    if (!string.IsNullOrWhiteSpace(dto.HrEmail))
                    {
                        ccList.Add(dto.HrEmail); // ✅ USER ENTERED EMAIL
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
                    x.Status == "Pending")
                .OrderBy(x => x.FromDate)
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
            return records.Count;
        }
    }
}
